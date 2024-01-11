#if DEBUG
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
                GameObject obj = new GameObject(typeof(DebugManager).Name);
                instance = obj.AddComponent<DebugManager>();
                DontDestroyOnLoad(obj);
            }

            return instance;
        }
    }
}
#endif