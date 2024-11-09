using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathUtil
{
    /// <summary>
    /// 根目录
    /// </summary>
    public static readonly string AssetsPath = Application.dataPath;

    /// <summary>
    /// 需要打bundle的目录
    /// </summary>
    public static readonly string BuildResourcesPath = AssetsPath + "/BuildResources/";

    /// <summary>
    /// bundle输出目录
    /// </summary>
    public static readonly string BundleOutPath = Application.streamingAssetsPath;//这个目录是只读的

    /// <summary>
    /// bundle资源路径
    /// </summary>
    public static string BundleResourcePath
    {//给版本信息.txt用的目录
        get { return Application.streamingAssetsPath; }//这个目录是只读的
    }

    /// <summary>
    /// 获取Unity的相对路径
    /// </summary>
    /// <param name="path">绝对路径</param>
    /// <returns>相对路径</returns>
    public static string GetUnityPath(string path)
    {
        if (string.IsNullOrEmpty(path))
            return string.Empty;
        return path.Substring(path.IndexOf("Assets"));
    }

    /// <summary>
    /// 获取标准路径
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
