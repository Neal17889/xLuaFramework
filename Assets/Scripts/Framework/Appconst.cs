﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
public enum GameMode
{
    EditorMode,
    PackgeBundle,
    UpdateMode,
}

public class AppConst
{
    public const string BundleExtension = ".ab";
    public const string FileListName = "filelist.txt";
    public static GameMode GameMode = GameMode.EditorMode;

    /// <summary>
    /// 热更资源地址
    /// </summary>
    public const string ResourcesUrl = "http://127.0.0.1/AssetBundles";
}