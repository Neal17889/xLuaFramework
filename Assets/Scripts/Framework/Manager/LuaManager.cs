using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using XLua;
/// <summary>
/// 异步加载
/// 同步使用
/// 预加载
/// </summary>
public class LuaManager : MonoBehaviour
{
    /// <summary>
    /// 所有的lua文件名
    /// </summary>
    public List<string> LuaNames = new List<string>();

    /// <summary>
    /// 缓存lua脚本内容
    /// </summary>
    private Dictionary<string, byte[]> m_LuaScripts;

    public LuaEnv LuaEnv;

    public void Init()
    {
        LuaEnv = new LuaEnv();
        LuaEnv.AddLoader(Loader);
        m_LuaScripts = new Dictionary<string, byte[]>();
        if (AppConst.GameMode != GameMode.EditorMode)
            LoadLuaScript();
#if UNITY_EDITOR
        if (AppConst.GameMode == GameMode.EditorMode)
            EditorLoadLuaScript();
#endif
    }

    /// <summary>
    /// Lua文件加载
    /// </summary>
    /// <param name="name"></param>
    public void StartLua(string name)
    {
        LuaEnv.DoString(string.Format("require '{0}'", name));
    }

    #region 自定义Loader
    byte[] Loader(ref string name)
    {
        return GetLuaScript(name);
    }

    public byte[] GetLuaScript(string name)
    {
        //require ui.login Lua中经常这么写，所以要将 . 换成路径中使用的 /
        name = name.Replace(".", "/");
        string fileName = PathUtil.GetLuaPath(name);

        byte[] luaScript = null;
        if (!m_LuaScripts.TryGetValue(fileName, out luaScript))
            Debug.LogErrorFormat("lua script is not exist : {0}", fileName);
        return luaScript;
    }
    #endregion

    /// <summary>
    /// 非编辑器模式Lua脚本预加载至内存
    /// </summary>
    void LoadLuaScript()
    {
        foreach (string name in LuaNames)
        {
            Manager.Resource.LoadLua(name, (UnityEngine.Object obj) =>
            {
                AddLuaScript(name, (obj as TextAsset).bytes);
                if (m_LuaScripts.Count >= LuaNames.Count)
                {
                    //所有lua加载完成之后
                    Manager.Event.Fire(10000);
                    LuaNames.Clear();
                    LuaNames = null;
                }
            });
        }
    }

    public void AddLuaScript(string assetsName, byte[] luaScript)
    {
        m_LuaScripts[assetsName] = luaScript;
    }

#if UNITY_EDITOR
    /// <summary>
    /// 编辑器模式Lua脚本预加载至内存
    /// </summary>
    void EditorLoadLuaScript()
    {
        string[] luaFiles = Directory.GetFiles(PathUtil.LuaPath, "*.bytes", SearchOption.AllDirectories);
        for (int i = 0; i < luaFiles.Length; i++)
        {
            string fileName = PathUtil.GetStandardPath(luaFiles[i]);
            byte[] file = File.ReadAllBytes(fileName);
            AddLuaScript(PathUtil.GetUnityPath(fileName), file);
        }
        Manager.Event.Fire(10000);
    }
#endif

    private void Update()
    {
        if (LuaEnv != null)
        {
            LuaEnv.Tick();//GC
        }
    }

    private void OnDestroy()
    {
        if (LuaEnv != null)
        {
            LuaEnv.Dispose();
            LuaEnv = null;
        }
    }
}
