using System;
using System.IO;
using System.Text;
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

    public delegate int OnSave(string filePath);
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

    public void Save()
    {
        onSave?.Invoke(filePath);
    }

#if UNITY_EDTIOR
    public void ExampleLoad(string fileName)
    {

    }

    public void ExampleSave(string fileName)
    {
        using (var stream = File.Open(fileName, FileMode.Create))
        {
            using (var writer = new BinaryWriter(stream, Encoding.UTF8, false))
            {
                writer.Write(1.250F);
                writer.Write(@"c:\Temp");
                writer.Write(10);
                writer.Write(true);
            }

            stream.Close();
        }
    }
#endif
}
