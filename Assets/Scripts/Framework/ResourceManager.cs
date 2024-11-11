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
    //存放bundle信息的集合
    private Dictionary<string, BundleInfo> m_BundleInfos = new Dictionary<string, BundleInfo>();
    /// <summary>
    /// 解析版本文件
    /// </summary>
    private void ParseVersionFile()
    {
        //版本文件的路径
        string url = Path.Combine(PathUtil.BundleResourcePath, AppConst.FileListName);
        string[] data = File.ReadAllLines(url);
        //解析文件信息
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
    /// 异步加载资源
    /// </summary>
    /// <param name="assetName">资源名</param>
    /// <param name="action">完成回调</param>
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
        Debug.LogFormat("LoadBundleAsync:{0}", assetName);
        action?.Invoke(assetBundleRequest?.asset);
    }

#if UNITY_EDITOR
    void EditorLoadAsset(string assetName, Action<UnityEngine.Object> action = null)
    {
        Debug.LogFormat("EditorLoadAsset:{0}", assetName);
        UnityEngine.Object obj = UnityEditor.AssetDatabase.LoadAssetAtPath(assetName, typeof(UnityEngine.Object));
        if (obj == null)
            Debug.LogErrorFormat("assetName is not exist:{0}", assetName);
        action?.Invoke(obj);
    }
#endif

    private void LoadAsset(string assetName, Action<UnityEngine.Object> action)
    {
#if UNITY_EDITOR
        EditorLoadAsset(assetName, action);
#endif
        if (AppConst.GameMode != GameMode.EditorMode)
            StartCoroutine(LoadBundleAsync(assetName, action));
    }

    public void LoadUI(string assetName, Action<UnityEngine.Object> action = null)
    {
        LoadAsset(PathUtil.GetUIPath(assetName), action);
    }

    public void LoadMusic(string assetName, Action<UnityEngine.Object> action = null)
    {
        LoadAsset(PathUtil.GetMusicPath(assetName), action);
    }

    public void LoadSound(string assetName, Action<UnityEngine.Object> action = null)
    {
        LoadAsset(PathUtil.GetSoundPath(assetName), action);
    }

    public void LoadEffect(string assetName, Action<UnityEngine.Object> action = null)
    {
        LoadAsset(PathUtil.GetEffectPath(assetName), action);
    }

    public void LoadScene(string assetName, Action<UnityEngine.Object> action = null)
    {
        LoadAsset(PathUtil.GetScenePath(assetName), action);
    }

    void Start()
    {
        ParseVersionFile();
        LoadUI("UILogin", OnComplete);
    }

    private void OnComplete(UnityEngine.Object @object)
    {
        GameObject go = Instantiate(@object) as GameObject;
        go.transform.SetParent(this.transform);
        go.SetActive(true);
        go.transform.localPosition = Vector3.zero;
    }
}
