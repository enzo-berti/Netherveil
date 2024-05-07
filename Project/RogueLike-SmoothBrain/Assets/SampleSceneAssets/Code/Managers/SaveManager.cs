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

    public delegate void OnSave(string directoryPath);
    public OnSave onSave; 

    private int selectedSave = -1;
    public string DirectoryPath { private set; get; } = string.Empty;

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

        if (!Directory.Exists(Application.persistentDataPath + "/Save"))
        {
            Directory.CreateDirectory(Application.persistentDataPath + "/Save");
        }
    }

    public void SelectSave(int selectedSave)
    {
        this.selectedSave = selectedSave;
        DirectoryPath = Application.persistentDataPath + "/Save/" + selectedSave.ToString();

        if (!Directory.Exists(DirectoryPath))
        {
            Directory.CreateDirectory(DirectoryPath);
        }
    }

    public void Save()
    {
        onSave?.Invoke(DirectoryPath);
    }

    const string exampleFile = "/Player.s";
    public void ExampleLoad()
    {
        string directoryPath = SaveManager.instance.DirectoryPath;

        if (File.Exists(directoryPath + exampleFile))
        {
            using (var stream = File.Open(directoryPath + exampleFile, FileMode.Open))
            {
                using (var reader = new BinaryReader(stream, Encoding.UTF8, false))
                {
                    Debug.Log(reader.ReadSingle());
                    Debug.Log(reader.ReadString());
                    Debug.Log(reader.ReadInt32());
                    Debug.Log(reader.ReadBoolean());
                }
            }
        }
    }

    public void ExampleSave(string directoryPath)
    {
        using (var stream = File.Open(directoryPath + exampleFile, FileMode.Create))
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
}
