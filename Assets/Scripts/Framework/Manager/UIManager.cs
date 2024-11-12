using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    //缓存UI，后续换成对象池
    Dictionary<string, GameObject> m_UI = new Dictionary<string, GameObject>();

    public void OpenUI(string uiName, string luaName)
    {
        GameObject ui = null;
        if (m_UI.TryGetValue(uiName, out ui))
        {
            UILogic uILogic = this.GetComponent<UILogic>();
            uILogic.Open();
            return;
        }
        Manager.Resource.LoadUI(uiName, (UnityEngine.Object obj) =>
        {
            GameObject ui = Instantiate(obj) as GameObject;
            m_UI.Add(uiName, ui);
            UILogic uILogic = ui.AddComponent<UILogic>();
            uILogic.Init(luaName);
            uILogic.Open();
        });
    }
}
