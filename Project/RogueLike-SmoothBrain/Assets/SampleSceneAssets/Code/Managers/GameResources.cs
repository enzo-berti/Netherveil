using System.Collections.Generic;
using UnityEngine;

public class GameResources : MonoBehaviour
{
    static private GameResources instance;
    private readonly Dictionary<string, Object> keyValuePairs = new Dictionary<string, Object>();

    static public GameResources Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject obj = new GameObject(nameof(GameResources));
                obj.AddComponent<GameResources>();
            }

            return instance;
        }
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(instance);
        }
        else
        {
            Destroy(instance);
            return;
        }
    }

    public T Get<T>(string key) where T : Object
    {
        if (!keyValuePairs.ContainsKey(key))
        {
            T obj = Resources.Load<T>(key);
            keyValuePairs.Add(key, obj);
        }

        return (T)keyValuePairs[key];
    }

    public bool Remove(string key)
    {
        return keyValuePairs.Remove(key);
    }
}