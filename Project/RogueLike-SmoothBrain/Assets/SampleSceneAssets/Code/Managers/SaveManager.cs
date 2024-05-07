using System;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveManager : MonoBehaviour
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void LoadSaveManager()
    {
        _ = Instance;
    }

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

    public delegate int OnSave(ref string saveContent);
    public OnSave onSave; 

    private int selectedSave = -1;
    private string filePath = string.Empty;

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

        if (!Directory.Exists(Application.persistentDataPath + "Save"))
        {
            Directory.CreateDirectory(Application.persistentDataPath + "Save");
        }

        SceneManager.sceneLoaded += CheckCanLoad;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= CheckCanLoad;
    }

    private void CheckCanLoad(Scene scene, LoadSceneMode loadSceneMode)
    {
        if (scene.buildIndex != SceneManager.GetSceneByName("InGame").buildIndex || selectedSave < 0)
        {
            return;
        }

        Load();
    }

    public void SelectSave(int selectedSave)
    {
        this.selectedSave = selectedSave;
        filePath = Application.persistentDataPath + "Save/" + "save" + selectedSave.ToString() + ".s";

        if (!File.Exists(filePath))
        {
            File.Create(filePath);
        }
    }

    private void Load()
    {
        Debug.Log("LOAD");
    }

    public void Save()
    {
        string saveContent = "";
        onSave?.Invoke(ref saveContent);

        File.WriteAllText(filePath, saveContent);
    }
}
