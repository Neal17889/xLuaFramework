using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    //缓存UI，后续换成对象池
    //Dictionary<string, GameObject> m_UI = new Dictionary<string, GameObject>();

    /// <summary>
    /// UI分组
    /// </summary>
    Dictionary<string, Transform> m_UIGroups = new Dictionary<string, Transform>();

    private Transform m_UIParent;

    private void Awake()
    {
        m_UIParent = this.transform.parent.Find("UI");
    }

    public void SetUIGroups(List<string> groups)
    {
        foreach (string group in groups)
        {
            GameObject go = new GameObject("Group-" +  group);
            go.transform.SetParent(m_UIParent, false);
            m_UIGroups.Add(group, go.transform);
        }
    }

    public Transform GetUIGroup(string group)
    {
        if (!m_UIGroups.ContainsKey(group))
        {
            Debug.LogErrorFormat("GetUIGroup:{0} is not found.", group);
            return null;
        }           
        else
            return m_UIGroups[group];
    }
    public void OpenUI(string uiName, string group, string luaName)
    {
        GameObject ui = null;
        Transform parent = GetUIGroup(group);
        string uiPath = PathUtil.GetUIPath(uiName);
        Object uiObj = Manager.Pool.Spawn("UI", uiPath);

        if (uiObj != null)
        {
            ui = uiObj as GameObject;
            ui.transform.SetParent(parent, false);
            UILogic uILogic = ui.GetComponent<UILogic>();
            uILogic.Open();
            
            return;
        }
        Manager.Resource.LoadUI(uiName, (UnityEngine.Object obj) =>
        {
            GameObject ui = Instantiate(obj) as GameObject;
            
            ui.transform.SetParent(parent, false);
            UILogic uILogic = ui.AddComponent<UILogic>();
            uILogic.AssetName = uiPath;
            uILogic.Init(luaName);
            uILogic.Open();
        });
    }
}
