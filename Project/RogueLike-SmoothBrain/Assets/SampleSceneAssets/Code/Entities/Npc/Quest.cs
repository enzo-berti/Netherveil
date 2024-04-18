using Generation;
using System;
using System.Reflection;
using UnityEngine;

public abstract class Quest
{
    public QuestData Datas { get; protected set; }
    public string progressText = string.Empty;
    static QuestDatabase database;
    public static event Action OnQuestUpdated;
    protected Hero player;
    protected QuestTalker.TalkerType talkerType;

    public abstract void AcceptQuest();

    static public Quest LoadClass(string name, QuestTalker.TalkerType type)
    {
        Quest quest = Assembly.GetExecutingAssembly().CreateInstance(name.GetPascalCase()) as Quest;
        quest.Datas = database.GetQuest(name);
        quest.player = GameObject.FindWithTag("Player").GetComponent<Hero>();
        quest.talkerType = type;
        return quest;
    }

    static public string GetRandomQuestName()
    {
        if (database == null)
        {
            database = GameResources.Get<QuestDatabase>("QuestDatabase");
        }

        int indexRandom = Seed.Range(0, database.datas.Count);
        return database.datas[indexRandom].idName;
    }

    protected virtual void QuestFinished()
    {
        player.CurrentQuest = null;
        player.Stats.IncreaseValue(Stat.CORRUPTION, talkerType == QuestTalker.TalkerType.CLERIC ? -Datas.CorruptionModifierValue : Datas.CorruptionModifierValue);
    }

    protected void QuestUpdated()
    {
        OnQuestUpdated?.Invoke();
    }
}
