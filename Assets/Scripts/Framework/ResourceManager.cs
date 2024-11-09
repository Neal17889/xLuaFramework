using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    internal class BundleInfo
    {
        public string AssetsName;
        public string BundleName;
        public List<string> Dependencie;
    }
    //���bundle��Ϣ�ļ���
    private Dictionary<string, BundleInfo> m_BundleInfos = new Dictionary<string, BundleInfo>();
    /// <summary>
    /// �����汾�ļ�
    /// </summary>
    private void ParseVersionFile()
    {
        //�汾�ļ���·��
        string url = Path.Combine(PathUtil.BundleResourcePath, AppConst.FileListName);
        string[] data = File.ReadAllLines(url);
        //�����ļ���Ϣ
        for (int i = 0; i < data.Length; i++)
        {
            BundleInfo bundleInfo = new BundleInfo();
            string[] info = data[i].Split('|');
            bundleInfo.AssetsName = info[0];
            bundleInfo.BundleName = info[1];
            bundleInfo.Dependencie = new List<string>(info.Length - 2);
            for (int j = 2; j < info.Length; j++)
            {
                bundleInfo.Dependencie.Add(info[j]);
            }
            m_BundleInfos.Add(bundleInfo.AssetsName, bundleInfo);
        }
    }

    /// <summary>
    /// �첽������Դ
    /// </summary>
    /// <param name="assetName">��Դ��</param>
    /// <param name="action">��ɻص�</param>
    /// <returns></returns>
    IEnumerator LoadBundleAsync(string assetName, Action<UnityEngine.Object> action = null)
    {
        string bundleName = m_BundleInfos[assetName].BundleName;
        string bundlePath = Path.Combine(PathUtil.BundleResourcePath, bundleName);
        List<string> dependencies = m_BundleInfos[assetName].Dependencie;
        if (dependencies != null && dependencies.Count > 0)
        {
            for (int i = 0;i < dependencies.Count;i++)
            {
                yield return LoadBundleAsync(dependencies[i]);
            }            
        }

        AssetBundleCreateRequest request = AssetBundle.LoadFromFileAsync(bundlePath);
        yield return request;

        AssetBundleRequest assetBundleRequest = request.assetBundle.LoadAssetAsync(assetName);
        yield return assetBundleRequest;

        action?.Invoke(assetBundleRequest?.asset);
    }

    public void LoadAsset(string assetName, Action<UnityEngine.Object> action)
    {
        StartCoroutine(LoadBundleAsync(assetName, action));
    }

    void Start()
    {
        ParseVersionFile();
        LoadAsset("Assets/BuildResources/UI/Prefab/UILogin.prefab", OnComplete);
    }

    private void OnComplete(UnityEngine.Object @object)
    {
        GameObject go = Instantiate(@object) as GameObject;
        go.transform.SetParent(this.transform);
        go.SetActive(true);
        go.transform.localPosition = Vector3.zero;
    }
}