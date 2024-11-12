using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityManager : MonoBehaviour
{
    Dictionary<string, GameObject> m_Entities = new Dictionary<string, GameObject>();
    Dictionary<string, Transform> m_Groups = new Dictionary<string, Transform>();
    private Transform m_EntityParent;
    private void Awake()
    {
        m_EntityParent = this.transform.parent.Find("Entity");
    }

    public void SetEntityGroups(List<string> groups)
    {
        for (int i = 0; i < groups.Count; i++)
        {
            GameObject group = new GameObject("Group-" +  groups[i]);
            group.transform.SetParent(m_EntityParent, false);
            m_Groups[groups[i]] = group.transform;
        }
    }

    public Transform GetEntityGroup(string group)
    {
        if (!m_Groups.ContainsKey(group))
        {
            Debug.LogErrorFormat("GetEntityGroup:{0} is not found", group);
            return null;
        }
        else
            return m_Groups[group];
    }

    public void ShowEntity(string name, string group, string luaName)
    {
        GameObject entity = null;
        if (m_Entities.TryGetValue(name, out entity))
        {
            EntityLogic logic = entity.GetComponent<EntityLogic>();
            logic.OnShow();
            return;
        }

        Manager.Resource.LoadPrefab(name, (UnityEngine.Object obj) =>
            {
                entity = Instantiate(obj) as GameObject;
                Transform parent = GetEntityGroup(group);
                entity.transform.SetParent(parent, false);
                m_Entities.Add(name, entity);
                EntityLogic entityLogic = entity.AddComponent<EntityLogic>();
                entityLogic.Init(luaName);
                entityLogic.OnShow();
            });
    }
}
