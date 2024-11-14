using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public class HotUpdate : MonoBehaviour
{
    byte[] m_ReadPathFileListData;
    byte[] m_ServerFileListData;
    private const int MaxRetryCount = 3;

    internal class DownFileInfo
    {
        public string url;
        public string fileName;
        public DownloadHandler fileData;
    }
    //�����ļ�����
    int m_DownloadCount;

    /// <summary>
    /// ���ص����ļ�
    /// </summary>
    /// <param name="info"></param>
    /// <param name="onComplete"></param>
    /// <returns></returns>
    IEnumerator DownloadFile(DownFileInfo info, Action<DownFileInfo> onComplete)
    {
        int retryCount = 0;
        UnityWebRequest webRequest;

        while (retryCount < MaxRetryCount)
        {
            webRequest = UnityWebRequest.Get(info.url);
            webRequest.useHttpContinue = true;
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.ConnectionError)
            {
                Debug.LogWarningFormat("Connection error occurred. Retrying ({0}/{1}): {2}", retryCount + 1, MaxRetryCount, info.url);
                retryCount++;

                if (retryCount < MaxRetryCount)
                {
                    yield return new WaitForSeconds(1); // �ȴ�1�������
                }
                else
                {
                    Debug.LogErrorFormat("Failed to download after {0} attempts: {1}", MaxRetryCount, info.url);
                }
            }
            else if (webRequest.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogErrorFormat("Protocol error occurred while downloading: {0}", info.url);
                break;
            }
            else if (webRequest.result == UnityWebRequest.Result.DataProcessingError)
            {
                Debug.LogErrorFormat("Data processing error occurred while downloading: {0}", info.url);
                break;
            }
            else if (webRequest.result == UnityWebRequest.Result.Success)
            {
                Debug.LogFormat("Successfully connected and downloaded from: {0}", info.url);
                yield return new WaitForSeconds(0.2f);
                info.fileData = webRequest.downloadHandler;
                onComplete?.Invoke(info);
                webRequest.Dispose();
                yield break; // ���سɹ����˳�Э��
            }
        }
    }

    /// <summary>
    /// ���ض���ļ�
    /// </summary>
    /// <param name="infos"></param>
    /// <param name="onComplete"></param>
    /// <param name="DownLoadAllComplete"></param>
    /// <returns></returns>
    IEnumerator DownloadFile(List<DownFileInfo> infos, Action<DownFileInfo> onComplete, Action DownLoadAllComplete)
    {
        foreach (DownFileInfo info in infos)
        {
            yield return DownloadFile(info, onComplete);
        }
        DownLoadAllComplete?.Invoke();
    }

    /// <summary>
    /// �����Ҫ���ص��ļ����б�
    /// </summary>
    /// <param name="fileData">filelist.txt�ļ����ַ���</param>
    /// <param name="path">��Ҫ���ص��ļ���url��ַ</param>
    /// <returns></returns>
    private List<DownFileInfo> GetFileList(string fileData, string path)
    {
        string content = fileData.Trim().Replace("\r", "");
        string[] files = content.Split('\n');
        List<DownFileInfo> downFileInfos = new List<DownFileInfo>(files.Length);
        for (int i = 0; i < files.Length; i++)
        {
            string[] info = files[i].Split('|');
            DownFileInfo fileInfo = new DownFileInfo();
            fileInfo.fileName = info[1];
            fileInfo.url = Path.Combine(path, info[1]);
            downFileInfos.Add(fileInfo);
        }
        return downFileInfos;
    }

    GameObject loadingObj;
    LoadingUI loadingUI;
    private void Start()
    {
        GameObject go = Resources.Load<GameObject>("LoadingUI");
        loadingObj = Instantiate(go);
        loadingObj.transform.SetParent(this.transform);
        loadingUI = loadingObj.GetComponent<LoadingUI>();

        if (IsFirstInstall())
        {
            ReleaseResources();
        }
        else
        {
            CheckUpdate();
        }
    }

    private bool IsFirstInstall()
    {
        //�ж�ֻ��Ŀ¼�Ƿ���ڰ汾�ļ�
        bool isExistsReadPath = FileUtil.IsExists(Path.Combine(PathUtil.ReadPath, AppConst.FileListName));

        //�жϿɶ�дĿ¼�Ƿ���ڰ汾�ļ�
        bool isExistsReadWritePath = FileUtil.IsExists(Path.Combine(PathUtil.ReadWritePath, AppConst.FileListName));

        return isExistsReadPath && !isExistsReadWritePath;
    }

    private void ReleaseResources()
    {
        m_DownloadCount = 0;
        string url = Path.Combine(PathUtil.ReadPath, AppConst.FileListName);
        DownFileInfo info = new() { url = url };
        StartCoroutine(DownloadFile(info, OnDownLoadReadPathFileListComplete));
    }

    private void OnDownLoadReadPathFileListComplete(DownFileInfo file)
    {
        m_ReadPathFileListData = file.fileData.data;
        List<DownFileInfo> fileInfos = GetFileList(file.fileData.text, PathUtil.ReadPath);
        StartCoroutine(DownloadFile(fileInfos, OnReleaseFileComplete, OnReleaseAllFileComplete));
        loadingUI.InitProgress(fileInfos.Count, "�����ͷ���Դ������������...");
    }

    private void OnReleaseFileComplete(DownFileInfo info)
    {
        Debug.LogFormat("OnReleaseFileComplete:{0}", info.url);
        string writePath = Path.Combine(PathUtil.ReadWritePath, info.fileName);
        FileUtil.WriteFile(writePath, info.fileData.data);
        m_DownloadCount++;
        loadingUI.UpdateProgress(m_DownloadCount);
    }

    private void OnReleaseAllFileComplete()
    {
        FileUtil.WriteFile(Path.Combine(PathUtil.ReadWritePath, AppConst.FileListName), m_ReadPathFileListData);
        CheckUpdate();
    }

    private void CheckUpdate()
    {
        string url = Path.Combine(AppConst.ResourcesUrl, AppConst.FileListName);
        DownFileInfo info = new() { url = url };
        StartCoroutine(DownloadFile(info, OnDownLoadServerFileListComplete));
    }

    private void OnDownLoadServerFileListComplete(DownFileInfo file)
    {
        m_DownloadCount = 0;
        m_ServerFileListData = file.fileData.data;
        List<DownFileInfo> fileInfos = GetFileList(file.fileData.text, AppConst.ResourcesUrl);
        List<DownFileInfo> downListFiles = new();

        for (int i = 0; i < fileInfos.Count; i++)
        {
            string localFile = Path.Combine(PathUtil.ReadWritePath, fileInfos[i].fileName);
            if (!FileUtil.IsExists(localFile))
            {
                downListFiles.Add(fileInfos[i]);
            }
        }
        if (downListFiles.Count > 0)
        {
            StartCoroutine(DownloadFile(fileInfos, OnUpdateFileComplete, OnUpdateAllFileComplete));
            loadingUI.InitProgress(downListFiles.Count, "���ڸ���...");
        }
        else
            EnterGame();
    }

    private void OnUpdateFileComplete(DownFileInfo info)
    {
        Debug.LogFormat("OnUpdateFileComplete:{0}", info.url);
        string writePath = Path.Combine(PathUtil.ReadWritePath, info.fileName);
        FileUtil.WriteFile(writePath, info.fileData.data);
        m_DownloadCount++;
        loadingUI.UpdateProgress(m_DownloadCount);
    }

    private void OnUpdateAllFileComplete()
    {
        FileUtil.WriteFile(Path.Combine(PathUtil.ReadWritePath, AppConst.FileListName), m_ServerFileListData);
        EnterGame();
        loadingUI.InitProgress(0, "��������...");
    }

    private void EnterGame()
    {
        Manager.Event.Fire((int)GameEvent.GameInit);
        Destroy(loadingObj);
        //Manager.Resource.ParseVersionFile();
        //Manager.Resource.LoadUI("UILogin", OnComplete);
    }

    //private void OnComplete(UnityEngine.Object @object)
    //{
    //    GameObject go = Instantiate(@object) as GameObject;
    //    go.transform.SetParent(this.transform);
    //    go.SetActive(true);
    //    go.transform.localPosition = Vector3.zero;
    //}
}
