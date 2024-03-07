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
                GameObject obj = new GameObject(typeof(GameAssets).Name);
                obj.AddComponent<GameAssets>();
            }

            return instance;
        }
    }

    public readonly Seed seed = new Seed();

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