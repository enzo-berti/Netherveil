#define DEBUG
//#define RELEASE

using UnityEngine;

public class GameManager : MonoBehaviour
{
    static private GameManager instance;
    static public GameManager Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject obj = new GameObject(typeof(GameManager).Name);
                instance = obj.AddComponent<GameManager>();
                DontDestroyOnLoad(obj);
            }

            return instance;
        }
    }

    public readonly Seed seed = new Seed();

    private void Instantiate()
    {
        seed.Generate();
    }

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instantiate();
    }
}