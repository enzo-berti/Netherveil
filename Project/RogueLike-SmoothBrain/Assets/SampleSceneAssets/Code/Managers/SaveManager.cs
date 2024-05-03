using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveManager : MonoBehaviour
{
    static private SaveManager instance;
    static public SaveManager Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject obj = new GameObject(nameof(SaveManager));
                obj.AddComponent<SaveManager>();
            }

            return instance;
        }
    }

    public Action onSave; 

    private int selectedSave = -1;

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

    private void OnLevelWasLoaded(int level)
    {
        if (level != SceneManager.GetSceneByName("InGame").buildIndex || selectedSave < 0)
        {
            return;
        }

        Load();
    }

    public bool SelectSave(int saveSelected)
    {

        return false;
    }

    private void Load()
    {

    }

    public void Save()
    {
        onSave?.Invoke();
    }
}
