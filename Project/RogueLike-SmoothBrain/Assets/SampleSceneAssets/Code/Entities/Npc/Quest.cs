using Generation;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public abstract class Quest
{
    public QuestData Datas { get; protected set; }
    static QuestDatabase database;

    public abstract void AcceptQuest();

    static public Quest LoadClass(string name)
    {
        QuestData data = database.GetQuest(name);
        Quest quest = Assembly.GetExecutingAssembly().CreateInstance(name.GetPascalCase()) as Quest;
        quest.Datas = data;
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
}
