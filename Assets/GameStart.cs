using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStart : MonoBehaviour
{
    public GameMode GameMode;
    private void Start()
    {
        Manager.Event.Subscribe(10000, OnLuaInit);

        AppConst.GameMode = this.GameMode;
        DontDestroyOnLoad(this);

        Manager.Resource.ParseVersionFile();//���߱༭��ģʽ����Ҫ
        Manager.Lua.Init();
    }

    void OnLuaInit(object args)
    {
        Manager.Lua.StartLua("main");
        //���������ԣ�ȫ�ֲ���Ч�ʺܵ�
        XLua.LuaFunction func = Manager.Lua.LuaEnv.Global.Get<XLua.LuaFunction>("Main");
        func.Call();

        Manager.Pool.CreateGameObjectPool("UI", 10);
        Manager.Pool.CreateGameObjectPool("Monster", 120);
        Manager.Pool.CreateGameObjectPool("Effect", 120);
        Manager.Pool.CreateAssetPool("AssetBundle", 10);
    }

    private void OnApplicationQuit()
    {
        Manager.Event.UnSubscribe(10000, OnLuaInit);
    }
}
