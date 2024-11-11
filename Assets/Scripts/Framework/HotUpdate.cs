using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public class HotUpdate : MonoBehaviour
{
    private const int MaxRetryCount = 3;

    internal class DownFileInfo
    {
        public string url;
        public string fileName;
        public DownloadHandler fileData;
    }

    /// <summary>
    /// 下载单个文件
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
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.ConnectionError)
            {
                Debug.LogWarningFormat("Connection error occurred. Retrying ({0}/{1}): {2}", retryCount + 1, MaxRetryCount, info.url);
                retryCount++;

                if (retryCount < MaxRetryCount)
                {
                    yield return new WaitForSeconds(1); // 等待1秒后重试
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
                info.fileData = webRequest.downloadHandler;
                onComplete?.Invoke(info);
                webRequest.Dispose();
                yield break; // 下载成功，退出循环
            }
        }
    }

    /// <summary>
    /// 下载多个文件
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
}
