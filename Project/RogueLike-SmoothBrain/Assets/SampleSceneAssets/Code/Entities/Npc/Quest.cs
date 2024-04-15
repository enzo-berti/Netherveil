using Generation;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public abstract class Quest
{
    public string idItemName;
    public string descriptionToDisplay;
    static QuestDatabase database;

    public abstract void AcceptQuest();

    static public Quest LoadClass(string name)
    {
        QuestData data = Quest.database.GetQuest(name);
        Quest quest = Assembly.GetExecutingAssembly().CreateInstance(name.GetPascalCase()) as Quest;
        quest.idItemName = data.idName;
        quest.descriptionToDisplay = data.Description;
        return quest;
    }

    static public string GetRandomQuestName()
    {
        if(database == null)
        {
            database = Resources.Load<QuestDatabase>("QuestDatabase");
        }

        List<string> allQuests = new();
        foreach (var questInDB in database.datas)
        {
            allQuests.Add(questInDB.idName);
        }
        int indexRandom = Seed.Range(0, allQuests.Count - 1);
        Debug.Log(allQuests[indexRandom]);
        return allQuests[indexRandom];
    }

    //static public void RandomizeQuest(Quest quest)
    //{
    //    List<string> allQuests = new();
    //    foreach (var questInDB in quest.database.datas)
    //    {
    //        allQuests.Add(questInDB.idName);
    //    }
    //    int indexRandom = Seed.Range(0, allQuests.Count - 1);
    //    quest.idItemName = allQuests[indexRandom];
    //}

    //public void RandomizeQuest()
    //{
    //    List<string> allQuests = new();
    //    foreach (var questsInDB in database.datas)
    //    {
    //        allQuests.Add(questsInDB.idName);
    //    }
    //    int indexRandom = UnityEngine.Random.Range(0, allQuests.Count - 1);
    //    idItemName = allQuests[indexRandom];
    //}
}
