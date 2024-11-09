using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathUtil
{
    /// <summary>
    /// ��Ŀ¼
    /// </summary>
    public static readonly string AssetsPath = Application.dataPath;

    /// <summary>
    /// ��Ҫ��bundle��Ŀ¼
    /// </summary>
    public static readonly string BuildResourcesPath = AssetsPath + "/BuildResources/";

    /// <summary>
    /// bundle���Ŀ¼
    /// </summary>
    public static readonly string BundleOutPath = Application.streamingAssetsPath;//���Ŀ¼��ֻ����

    /// <summary>
    /// bundle��Դ·��
    /// </summary>
    public static string BundleResourcePath
    {//���汾��Ϣ.txt�õ�Ŀ¼
        get { return Application.streamingAssetsPath; }//���Ŀ¼��ֻ����
    }

    /// <summary>
    /// ��ȡUnity�����·��
    /// </summary>
    /// <param name="path">����·��</param>
    /// <returns>���·��</returns>
    public static string GetUnityPath(string path)
    {
        if (string.IsNullOrEmpty(path))
            return string.Empty;
        return path.Substring(path.IndexOf("Assets"));
    }

    /// <summary>
    /// ��ȡ��׼·��
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static string GetStandardPath(string path)
    {
        if (string.IsNullOrEmpty(path))
            return string.Empty;
        return path.Trim().Replace("\\", "/");
    }
}
