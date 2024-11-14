using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStart : MonoBehaviour
{
    public GameMode GameMode;
    public bool OpenLog;
    // Start is called before the first frame update
    void Start()
    {
        Manager.Event.Subscribe((int)GameEvent.StartLua, StartLua);
        Manager.Event.Subscribe((int)GameEvent.GameInit, GameInit);
        AppConst.GameMode = this.GameMode;
        AppConst.OpenLog = this.OpenLog;
        DontDestroyOnLoad(this);

        if (AppConst.GameMode == GameMode.UpdateMode)
            this.gameObject.AddComponent<HotUpdate>();
        else
            Manager.Event.Fire((int)GameEvent.GameInit);
    }
	
	private void GameInit(object args)
    {
        if (AppConst.GameMode != GameMode.EditorMode)
            Manager.Resource.ParseVersionFile();
        Manager.Lua.Init();
    }

    private void StartLua(object args)
    {
        Manager.Lua.StartLua("main");
        XLua.LuaFunction func = Manager.Lua.LuaEnv.Global.Get<XLua.LuaFunction>("Main");
        func.Call();
        Manager.Pool.CreateGameObjectPool("UI", 10);
        Manager.Pool.CreateGameObjectPool("Monster", 120);
        Manager.Pool.CreateGameObjectPool("Effect", 120);
        Manager.Pool.CreateAssetPool("AssetBundle", 10);
    }

    


    private void OnApplicationQuit()
    {
        Manager.Event.UnSubscribe((int)GameEvent.StartLua, StartLua);
        Manager.Event.UnSubscribe((int)GameEvent.GameInit, GameInit);
    }
}

