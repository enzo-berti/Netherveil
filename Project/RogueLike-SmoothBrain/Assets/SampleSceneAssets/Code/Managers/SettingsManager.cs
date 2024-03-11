using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class SettingsManager : MonoBehaviour
{
    static SettingsManager instance;
    static public SettingsManager Instance
    {
        get
        {
            if(instance == null)
            {
                throw new System.Exception("ADD THE PREFAB TO YOUR SCENE SCUMBAG");
            }

            return instance;
        }
    }

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Debug.LogWarning("Try to create a second SettingsManager", gameObject);
            Destroy(gameObject);
            return;
        }
    }
}
