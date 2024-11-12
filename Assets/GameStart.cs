using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStart : MonoBehaviour
{
    public GameMode GameMode;
    private void Start()
    {
        AppConst.GameMode = this.GameMode;
        DontDestroyOnLoad(this);

        Manager.Resource.ParseVersionFile();//不走编辑器模式才需要
        Manager.Lua.Init(
            ()=>
            {
                Manager.Lua.StartLua("main");
                //仅用作测试，全局查找效率很低
                XLua.LuaFunction func = Manager.Lua.LuaEnv.Global.Get<XLua.LuaFunction>("Main");
                func.Call();
            }
            );
        

        
    }
}
