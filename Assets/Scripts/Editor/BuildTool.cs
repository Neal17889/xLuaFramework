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
        //�ļ���Ϣ�б�
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

            #region �����汾��Ϣ(�ļ�·����|bundle��|�����ļ��б�.txt)
            //����ļ���������Ϣ
            List<string> dependenceInfo = GetDependencies(assetName);
            StringBuilder sb = new StringBuilder();

            // ��ʼ�� `bundleInfo`
            sb.Append(assetName);
            sb.Append("|");
            sb.Append(bundleName);
            sb.Append(".ab");

            // ������������ÿ���������� `|` ����
            if (dependenceInfo.Count > 0)
            {
                sb.Append("|");
                foreach (string dependency in dependenceInfo)
                {
                    sb.Append(dependency);
                    sb.Append("|");
                }
                // ɾ�����һ������� `|`
                sb.Length -= 1;
            }

            // ���ս��
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
    /// ��ȡ������
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
