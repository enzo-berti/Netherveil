using UnityEngine;

public class GameAssets : MonoBehaviour
{
    static private GameAssets instance;
    static public GameAssets Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject obj = new GameObject(nameof(GameAssets));
                obj.AddComponent<GameAssets>();
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
}