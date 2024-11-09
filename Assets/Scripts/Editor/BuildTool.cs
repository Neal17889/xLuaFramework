using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Linq;
using System.Text;

public class BuildTool : Editor
{
    [MenuItem("Tools/Build Windows Bundle")]
    static void BuildWindowsBundle()
    {
        Build(BuildTarget.StandaloneWindows);
    }

    [MenuItem("Tools/Build Android Bundle")]
    static void BuildAndroidBundle()
    {
        Build(BuildTarget.Android);
    }

    [MenuItem("Tools/Build iPhone Bundle")]
    static void BuildiPhoneBundle()
    {
        Build(BuildTarget.iOS);
    }
    static void Build(BuildTarget target)
    {
        List<AssetBundleBuild> assetBundleBuilds = new List<AssetBundleBuild>();
        //文件信息列表
        List<string> bundleInfos = new List<string>();
        string[] files = Directory.GetFiles(PathUtil.BuildResourcesPath, "*", SearchOption.AllDirectories);
        for (int i = 0; i < files.Length; i++)
        {
            if (files[i].EndsWith(".meta"))
            {
                continue;
            }
            Debug.LogFormat("File:{0}", files[i]);
            AssetBundleBuild assetBundleBuild = new AssetBundleBuild();
            string fileName = PathUtil.GetStandardPath(files[i]);
            Debug.LogFormat("File:{0}", fileName);
            string assetName = PathUtil.GetUnityPath(fileName);
            assetBundleBuild.assetNames = new string[] { assetName };
            string bundleName = fileName.Replace(PathUtil.BuildResourcesPath, "").ToLower();
            assetBundleBuild.assetBundleName = bundleName + ".ab";
            assetBundleBuilds.Add(assetBundleBuild);

            #region 构建版本信息(文件路径名|bundle名|依赖文件列表.txt)
            //添加文件和依赖信息
            List<string> dependenceInfo = GetDependencies(assetName);
            StringBuilder sb = new StringBuilder();

            // 初始化 `bundleInfo`
            sb.Append(assetName);
            sb.Append("|");
            sb.Append(bundleName);
            sb.Append(".ab");

            // 如果有依赖项，将每个依赖项用 `|` 连接
            if (dependenceInfo.Count > 0)
            {
                sb.Append("|");
                foreach (string dependency in dependenceInfo)
                {
                    sb.Append(dependency);
                    sb.Append("|");
                }
                // 删除最后一个多余的 `|`
                sb.Length -= 1;
            }

            // 最终结果
            string bundleInfo = sb.ToString();

            bundleInfos.Add(bundleInfo);
            #endregion
        }

        if (Directory.Exists(PathUtil.BundleOutPath))
        {
            Directory.Delete(PathUtil.BundleOutPath, true);
        }
        Directory.CreateDirectory(PathUtil.BundleOutPath);

        File.WriteAllLines(Path.Combine(PathUtil.BundleOutPath, AppConst.FileListName), bundleInfos);

        BuildPipeline.BuildAssetBundles(PathUtil.BundleOutPath, assetBundleBuilds.ToArray(), BuildAssetBundleOptions.None, target);

        AssetDatabase.Refresh();
    }

    /// <summary>
    /// 获取依赖项
    /// </summary>
    /// <param name="currentFile"></param>
    /// <returns></returns>
    static List<string> GetDependencies(string currentFile)
    {
        List<string> dependencies = new List<string>();
        string[] files = AssetDatabase.GetDependencies(currentFile);
        dependencies = files.Where(file => !file.EndsWith(".cs") && !file.Equals(currentFile)).ToList();
        return dependencies;
    }
}
