using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

public struct SaveData
{
    // Player
    public string name;
    public string seed;
    // Map
    public int seedIteration;
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

    public bool hasData;

    private void Reset()
    {
        name = "Hero";
        seed = string.Empty;

        seedIteration = 0;

        doneQuestQThisStage = false;
        doneQuestQTApprenticeThisStage = false;
        clearedTuto = false;

        activeItemName = string.Empty;
        activeItemCooldown = 0f;
        passiveItemNames = new List<string>();
        bloodValue = 0;

        statHp = 0f;
        statCorruption = 0f;

        questId = string.Empty;
        questTimer = 0f;
        questDifficulty = Quest.QuestDifficulty.EASY;
        talkerType = QuestTalker.TalkerType.SHAMAN;
        talkerGrade = QuestTalker.TalkerGrade.APPRENTICE;
        questEvolution = 0;
    }

    public readonly void Save(string filePath)
    {
        using var stream = File.Open(filePath, FileMode.Create);
        using var writer = new BinaryWriter(stream, Encoding.UTF8, false);

        Debug.Log(name + " " + filePath);

        // Player
        writer.Write(name);
        writer.Write(seed);
        // Map
        writer.Write(seedIteration);
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
        hasData = false;
        Reset();
        if (!File.Exists(filePath))
        {
            return;
        }

        using var stream = File.Open(filePath, FileMode.Open);
        using var reader = new BinaryReader(stream, Encoding.UTF8, false);

        // Player
        name = reader.ReadString();
        seed = reader.ReadString();
        // Map
        seedIteration = reader.ReadInt32();
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

        hasData = true;
    }
}

static public class SaveManager
{
    public delegate void OnSave(ref SaveData saveData);
    static public event OnSave onSave;

    static public string FilePath { private set; get; } = string.Empty;
    static public SaveData saveData;

    static public void EraseSave()
    {

    }

    static public void SelectSave(int selectedSave)
    {
        Debug.Log(selectedSave);
        FilePath = Application.persistentDataPath + "/Save/" + selectedSave.ToString();

        if (!Directory.Exists(Application.persistentDataPath + "/Save"))
        {
            Directory.CreateDirectory(Application.persistentDataPath + "/Save");
        }

        Load();
    }

    static private void Load()
    {
        try
        {
            saveData.Load(FilePath);
        }
        catch (Exception e)
        {
            Debug.LogException(e);

            File.Delete(FilePath);
        }
    }

    static public void Save()
    {
        if (FilePath == string.Empty)
        {
            return;
        }

        onSave?.Invoke(ref saveData);

        try
        {
            saveData.Save(FilePath);
        }
        catch (Exception e)
        {
            Debug.LogException(e);

            File.Delete(FilePath);
        }
    }
}
