using System.IO;
using System.Linq;
using UnityEngine;

static public class SaveManager
{
    public delegate void OnSave(string directoryPath);
    static public event OnSave onSave;

    static public string DirectoryPath { private set; get; } = string.Empty;
    static public bool HasData { private set; get; } = false;

    static public void SelectSave(int selectedSave)
    {
        DirectoryPath = Application.persistentDataPath + "/Save/" + selectedSave.ToString() + "/";

        if (!Directory.Exists(Application.persistentDataPath + "/Save"))
        {
            Directory.CreateDirectory(Application.persistentDataPath + "/Save");
        }

        if (!Directory.Exists(DirectoryPath))
        {
            Directory.CreateDirectory(DirectoryPath);
        }

        // temporaire
        // c'est pas genial il faudrait faire un check de si les fichiers sont vraiment complet ou pas avant de vouloir charger une partie
        HasData = new DirectoryInfo(DirectoryPath).GetFiles().Any(); // check if save has files or not
    }

    /// <summary>
    /// Erase all the file in the selectionned save folder
    /// </summary>
    static public void NukeSave()
    {
        DirectoryInfo directory = new DirectoryInfo(DirectoryPath);

        foreach (FileInfo file in directory.GetFiles())
        {
            file.Delete();
        }
    }

    static public void Save()
    {
        if (DirectoryPath == string.Empty)
        {
            return;
        }

        onSave?.Invoke(DirectoryPath);
    }

    //const string filePathExample = "/Player.s";
    //public void ExampleLoad()
    //{
    //    string directoryPath = SaveManager.instance.DirectoryPath;
    //
    //    if (File.Exists(directoryPath + filePath))
    //    {
    //        using (var stream = File.Open(directoryPath + filePath, FileMode.Open))
    //        {
    //            using (var reader = new BinaryReader(stream, Encoding.UTF8, false))
    //            {
    //                Debug.Log(reader.ReadSingle());
    //                Debug.Log(reader.ReadString());
    //                Debug.Log(reader.ReadInt32());
    //                Debug.Log(reader.ReadBoolean());
    //            }
    //        }
    //    }
    //}
    //
    //public void ExampleSave(string directoryPath)
    //{
    //    using (var stream = File.Open(directoryPath + filePath, FileMode.Create))
    //    {
    //        using (var writer = new BinaryWriter(stream, Encoding.UTF8, false))
    //        {
    //            writer.Write(1.250F);
    //            writer.Write(@"c:\Temp");
    //            writer.Write(10);
    //            writer.Write(true);
    //        }
    //
    //        stream.Close();
    //    }
    //}
}
