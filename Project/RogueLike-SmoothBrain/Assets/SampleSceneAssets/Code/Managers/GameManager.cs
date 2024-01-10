using UnityEngine;

public class GameManager : MonoBehaviour
{
    private GameManager instance;

    public GameManager Instance 
    { 
        get 
        { 
            if (instance == null)
            {
                GameObject obj = new GameObject(GetType().Name);
                instance = obj.AddComponent<GameManager>();
                DontDestroyOnLoad(obj);
            }

            return instance;
        } 
    }
}
