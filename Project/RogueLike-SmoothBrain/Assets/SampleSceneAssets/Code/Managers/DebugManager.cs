#if UNITY_EDITOR || DEVELOPMENT_BUILD
using UnityEngine;

public class DebugManager : MonoBehaviour
{
    static private DebugManager instance;
    static public DebugManager Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject obj = new GameObject(nameof(DebugManager));
                instance = obj.AddComponent<DebugManager>();
                DontDestroyOnLoad(obj);
            }

            return instance;
        }
    }
}
#endif