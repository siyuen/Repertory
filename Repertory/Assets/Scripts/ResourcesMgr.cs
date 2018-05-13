using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourcesMgr : MonoBehaviour {

    private const string NAME = "ResourcesMgr";
    private Dictionary<string, List<GameObject>> goPool = new Dictionary<string, List<GameObject>>();
    private static ResourcesMgr instance;
    private static object syncRoot = new object();

    public static ResourcesMgr Instance()
    {
        if (instance)
            return instance;

        lock (syncRoot)
        {
            if (instance == null)
            {
                ResourcesMgr[] instances = FindObjectsOfType<ResourcesMgr>();
                if (instances != null)
                {
                    for (int i = 0; i < instances.Length; i++)
                        Destroy(instances[i]);
                    GameObject go = new GameObject(NAME);
                    instance = go.AddComponent<ResourcesMgr>();
                    DontDestroyOnLoad(instance);
                }
            }
        }
        return instance;
    }

    /// <summary>
    /// 获取资源
    /// </summary>
    public GameObject PopPool(string name)
    {
        if (!goPool.ContainsKey(name))
        {
            goPool.Add(name, new List<GameObject>());
            GameObject go = Instantiate(Resources.Load<GameObject>(name));
            return go;
        }
        else if (goPool[name].Count == 0)
        {
            GameObject go = Instantiate(Resources.Load<GameObject>(name));
            return go;
        }
        else
        {
            GameObject go = goPool[name][0];
            goPool[name].RemoveAt(0);
            return go;
        }
    }

    /// <summary>
    /// 放入资源
    /// </summary>
    public void PushPool(string name, GameObject go)
    {
        if (!goPool.ContainsKey(name))
        {
            goPool.Add(name, new List<GameObject>());
            goPool[name].Add(go);
        }
        else if (goPool[name] == null)
        {
            goPool[name] = new List<GameObject>();
            goPool[name].Add(go);
        }
        else
        {
            goPool[name].Add(go);
        }
    }
}
