using Generation;
using System.Reflection;
using UnityEngine;

public abstract class Quest
{
    public QuestData Datas { get; protected set; }
    public string progressText = string.Empty;
    static QuestDatabase database;

    public abstract void AcceptQuest();

    static public Quest LoadClass(string name)
    {
        Quest quest = Assembly.GetExecutingAssembly().CreateInstance(name.GetPascalCase()) as Quest;
        quest.Datas = database.GetQuest(name);
        return quest;
    }

    static public string GetRandomQuestName()
    {
        if(database == null)
        {
            database = Resources.Load<QuestDatabase>("QuestDatabase");
        }

        int indexRandom = Seed.Range(0, database.datas.Count);
        return database.datas[indexRandom].idName;
    }

    protected void QuestFinished()
    {
        GameObject.FindWithTag("Player").GetComponent<Hero>().CurrentQuest = null;
    }
}
