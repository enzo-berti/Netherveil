using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

public struct SaveData
{
    // Player
    public string Name;
    public string Seed;
    // Map
    public int SeedIteration;
    // Hero
    public bool doneQuestQThisStage;
    public bool doneQuestQTApprenticeThisStage;
    public bool clearedTuto;
    // Hero Items
    public string activeItemName;
    public float activeItemCooldown;
    public List<string> passiveItemNames;
    public int bloodValue;
    // Hero Stats
    public float statHp;
    public float statCorruption;
    // Hero Quest
    public string questId;
    public float questTimer;
    public Quest.QuestDifficulty questDifficulty;
    public QuestTalker.TalkerType talkerType;
    public QuestTalker.TalkerGrade talkerGrade;
    public int questEvolution;

    public readonly void Save(string filePath)
    {
        using var stream = File.Open(filePath, FileMode.Create);
        using var writer = new BinaryWriter(stream, Encoding.UTF8, false);

        // Player
        writer.Write(Name);
        writer.Write(Seed);
        // Map
        writer.Write(SeedIteration);
        // Hero
        writer.Write(doneQuestQThisStage);
        writer.Write(doneQuestQTApprenticeThisStage);
        writer.Write(clearedTuto);
        // Hero activeItem
        writer.Write(activeItemName.Any());
        if (activeItemName.Any())
        {
            writer.Write(activeItemName);
            writer.Write(activeItemCooldown);
        }
        // Hero passiveItems
        writer.Write(passiveItemNames.Count);
        foreach (var itemName in passiveItemNames)
        {
            writer.Write(itemName);
        }
        writer.Write(bloodValue);
        // Hero Stats
        writer.Write(statHp);
        writer.Write(statCorruption);
        // Hero Quest
        writer.Write(questId.Any());
        if (questId.Any())
        {
            writer.Write(questId);
            writer.Write(questTimer);
            writer.Write(questEvolution);
            writer.Write(questDifficulty.ToString());
            writer.Write(talkerType.ToString());
            writer.Write(talkerGrade.ToString());
        }
    }

    public void Load(string filePath)
    {
        using var stream = File.Open(filePath, FileMode.Open);
        using var reader = new BinaryReader(stream, Encoding.UTF8, false);

        // Player
        Name = reader.ReadString();
        Seed = reader.ReadString();
        // Map
        SeedIteration = reader.ReadInt32();
        // Hero
        doneQuestQThisStage = reader.ReadBoolean();
        doneQuestQTApprenticeThisStage = reader.ReadBoolean();
        clearedTuto = reader.ReadBoolean();
        // Hero activeItem
        if (reader.ReadBoolean())
        {
            activeItemName = reader.ReadString();
            activeItemCooldown = reader.ReadSingle();
        }
        // Hero passiveItems
        int itemCounts = reader.ReadInt32();
        passiveItemNames = new List<string>();
        for (int i = 0; i < itemCounts; i++)
        {
            passiveItemNames.Add(reader.ReadString());
        }
        bloodValue = reader.ReadInt32();
        // Hero Stats
        statHp = reader.ReadSingle();
        statCorruption = reader.ReadSingle();
        // Hero Quest
        if (reader.ReadBoolean())
        {
            questId = reader.ReadString();
            questTimer = reader.ReadSingle();
            questEvolution = reader.ReadInt32();
            questDifficulty = (Quest.QuestDifficulty)Enum.Parse(typeof(Quest.QuestDifficulty), reader.ReadString(), true);
            talkerType = (QuestTalker.TalkerType)Enum.Parse(typeof(QuestTalker.TalkerType), reader.ReadString());
            talkerGrade = (QuestTalker.TalkerGrade)Enum.Parse(typeof(QuestTalker.TalkerGrade), reader.ReadString());
        }
    }
}

static public class SaveManager
{
    public delegate void OnSave(ref SaveData saveData);
    static public event OnSave onSave;

    static public string DirectoryPath { private set; get; } = string.Empty;
    static public bool HasData { private set; get; } = false;
    static public SaveData saveData;

    static public void UnselectSave()
    {

    }

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

        try
        {
            saveData.Load(DirectoryPath);
        }
        catch (Exception e)
        {
            Debug.LogException(e);

            HasData = false; // can't load correctly the datas
            // TODO : destroy corrupted datas
        }
    }

    static public void Save()
    {
        if (DirectoryPath == string.Empty)
        {
            return;
        }

        onSave?.Invoke(ref saveData);

        try
        {
            saveData.Save(DirectoryPath);
        }
        catch (Exception e)
        {
            Debug.LogException(e);

            // TODO : destroy corrupted datas
        }
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
