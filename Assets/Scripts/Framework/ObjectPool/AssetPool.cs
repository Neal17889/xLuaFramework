using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssetPool : PoolBase
{
    public override UnityEngine.Object Spwan(string name)
    {
        return base.Spwan(name);
    }

    public override void UnSpwan(string name, UnityEngine.Object obj)
    {
        base.UnSpwan(name, obj);
    }

    public override void Release()
    {
        base.Release();
        foreach (PoolObject item in m_Objects)
        {
            if (System.DateTime.Now.Ticks - item.LastUseTime.Ticks >= m_ReleaseTime * 10000000)
            {
                Debug.Log("AssetPool release  time:" + System.DateTime.Now + "unload ab :" + item.Name);
                Manager.Resource.UnloadBundle(item.Object);
                m_Objects.Remove(item);
                Release();
                return;
            }
        }
    }
}
