using Generation;
using System;
using System.Reflection;
using UnityEngine;

public abstract class Quest
{
    public enum QuestDifficulty
    {
        EASY,
        MEDIUM,
        HARD,
        NB
    }

    public QuestData Datas { get; protected set; }
    public string progressText = string.Empty;
    static QuestDatabase database;
    public static event Action OnQuestUpdated;
    protected Hero player;
    protected QuestTalker.TalkerType talkerType;
    protected QuestTalker.TalkerGrade talkerGrade;
    protected QuestDifficulty difficulty;

    public abstract void AcceptQuest();

    static public Quest LoadClass(string name, QuestTalker.TalkerType type, QuestTalker.TalkerGrade grade)
    {
        Quest quest = Assembly.GetExecutingAssembly().CreateInstance(name.GetPascalCase()) as Quest;
        quest.Datas = database.GetQuest(name);
        quest.player = GameObject.FindWithTag("Player").GetComponent<Hero>();
        quest.talkerType = type;
        quest.talkerGrade = grade;
        quest.difficulty = (QuestDifficulty)Seed.Range(0, (int)QuestDifficulty.NB);
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
        if(talkerGrade == QuestTalker.TalkerGrade.BOSS)
        {
            player.GetComponent<PlayerController>().DoneQuestQTThiStage = true;
        }
        else
        {
            player.GetComponent<PlayerController>().DoneQuestQTApprenticeThiStage = true;
        }
        player.Stats.IncreaseValue(Stat.CORRUPTION, talkerType == QuestTalker.TalkerType.CLERIC ? -Datas.CorruptionModifierValue : Datas.CorruptionModifierValue);
    }

    protected virtual void QuestLost()
    {
        player.CurrentQuest = null;
        //add feedback to show that quest is lost
    }

    protected void QuestUpdated()
    {
        OnQuestUpdated?.Invoke();
    }
}
