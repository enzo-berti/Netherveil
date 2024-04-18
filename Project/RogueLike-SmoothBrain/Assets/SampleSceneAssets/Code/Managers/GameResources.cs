using System.Collections.Generic;
using UnityEngine;

public class GameResources : MonoBehaviour
{
    static private GameResources instance;
    private readonly Dictionary<string, Object> keyValuePairs = new Dictionary<string, Object>();
    [SerializeField] private List<Object> objectsToLoad;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void LoadGameResources()
    {
        _ = Instance;
    }

    static public GameResources Instance
    {
        get
        {
            if (instance == null)
            {
                Instantiate(Resources.Load<GameObject>(nameof(AudioManager)));
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

        foreach (Object obj in objectsToLoad)
        {
            Debug.Log(obj.name);
            keyValuePairs.Add(obj.name, obj);
        }
        objectsToLoad.Clear();
    }

    public T Get<T>(string key) where T : Object
    {
        if (!keyValuePairs.ContainsKey(key))
        {
            T obj = Resources.Load<T>(key);
            if (obj == null)
            {
                Debug.LogError("GameResources doesn't contain " + key + " and can't load this from file");
            }
            keyValuePairs.Add(key, obj);
        }

        return (T)keyValuePairs[key];
    }

    public bool Remove(string key)
    {
        return keyValuePairs.Remove(key);
    }
}