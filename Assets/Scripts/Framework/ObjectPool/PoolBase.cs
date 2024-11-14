using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolBase : MonoBehaviour
{
    //自动释放时间/秒
    protected float m_ReleaseTime;

    //上次释放资源的时间/毫微秒   1(秒) = 10000000(毫微秒)
    protected long m_LastReleaseTime = 0;

    //真正的对象池
    protected List<PoolObject> m_Objects;

    public void Start()
    {
        m_LastReleaseTime = System.DateTime.Now.Ticks;
    }

    //初始化对象池
    public void Init(float time)
    {
        m_ReleaseTime = time;
        m_Objects = new List<PoolObject>();
    }

    //取出对象
    public virtual Object Spwan(string name)
    {
        foreach (PoolObject po in m_Objects)
        {
            if (po.Name == name)
            {
                m_Objects.Remove(po);
                return po.Object;
            }
        }
        return null;
    }

    //回收对象
    public virtual void UnSpwan(string name, Object obj)
    {
        PoolObject po = new PoolObject(name, obj);
        m_Objects.Add(po);
    }

    public virtual void Release()
    {

    }

    void Update()
    {
        if (System.DateTime.Now.Ticks - m_LastReleaseTime >= m_ReleaseTime * 10000000)
        {
            m_LastReleaseTime = System.DateTime.Now.Ticks;
            Release();
        }
    }
}
