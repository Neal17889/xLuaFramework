using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    public delegate void EnventHandler(object args);

    Dictionary<int, EnventHandler> m_Events = new Dictionary<int, EnventHandler>();

    public void Subscribe(int id, EnventHandler e)
    {
        if (m_Events.ContainsKey(id))
            m_Events[id] += e;
        else
            m_Events.Add(id, e);
    }

    public void UnSubscribe(int id, EnventHandler e)
    {
        if (m_Events.ContainsKey(id))
        {
            if (m_Events[id] != null)
                m_Events[id] -= e;

            if (m_Events[id] == null)
                m_Events.Remove(id);
        }
    }

    public void Fire(int id, object args = null)
    {
        EnventHandler handler;
        if (m_Events.TryGetValue(id, out handler))
        {
            handler(args);
        }
    }
}
