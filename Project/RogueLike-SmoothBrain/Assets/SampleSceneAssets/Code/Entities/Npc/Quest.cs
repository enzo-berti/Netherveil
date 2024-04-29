using Map;
using Map.Generation;
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
    protected bool questLost = false;

    public virtual void AcceptQuest()
    {
        AudioManager.Instance.PlaySound(AudioManager.Instance.QuestObtainedSFX);
        RoomUtilities.onAllEnemiesDead += CheckQuestFinished;
        RoomUtilities.onAllChestOpen += CheckQuestFinished;
        RoomUtilities.onEnter += CheckQuestFinished;
    }

    static public Quest LoadClass(string name, QuestTalker.TalkerType type, QuestTalker.TalkerGrade grade)
    {
        Quest quest = Assembly.GetExecutingAssembly().CreateInstance(name.GetPascalCase()) as Quest;
        quest.Datas = database.GetQuest(name);
        quest.player = GameObject.FindWithTag("Player").GetComponent<Hero>();
        quest.talkerType = type;
        quest.talkerGrade = grade;
        quest.difficulty = quest.Datas.HasDifferentGrades ? (QuestDifficulty)Seed.Range(0, (int)QuestDifficulty.NB) : QuestDifficulty.MEDIUM;
        InitDescription(ref quest.Datas.Description);
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
        AudioManager.Instance.PlaySound(AudioManager.Instance.QuestFinishedSFX);
        player.CurrentQuest = null;
        if (talkerGrade == QuestTalker.TalkerGrade.BOSS)
        {
            player.GetComponent<PlayerController>().DoneQuestQTThiStage = true;
        }
        else
        {
            player.GetComponent<PlayerController>().DoneQuestQTApprenticeThiStage = true;
        }
        player.Stats.IncreaseValue(Stat.CORRUPTION, talkerType == QuestTalker.TalkerType.CLERIC ? -Datas.CorruptionModifierValue : Datas.CorruptionModifierValue);

        RoomUtilities.onAllEnemiesDead -= CheckQuestFinished;
        RoomUtilities.onAllChestOpen -= CheckQuestFinished;
        RoomUtilities.onEnter -= CheckQuestFinished;
    }

    protected void CheckQuestFinished()
    {
        if (questLost)
        {
            QuestLost();
            HudHandler.current.MessageInfoHUD.Display($"You lost the quest \"{Datas.idName.SeparateAllCase()}\".");
        }
        else if (IsQuestFinished())
        {
            QuestFinished();
            HudHandler.current.MessageInfoHUD.Display($"You finished the quest \"{Datas.idName.SeparateAllCase()}\".");
        }
    }

    protected virtual void QuestLost()
    {
        AudioManager.Instance.PlaySound(AudioManager.Instance.QuestLostSFX);
        player.CurrentQuest = null;
        //add feedback to show that quest is lost
    }

    protected abstract bool IsQuestFinished();

    protected void QuestUpdated()
    {
        OnQuestUpdated?.Invoke();
    }
    static private void InitDescription(ref string description)
    {
        string finalDescription = string.Empty;
        char[] separators = new char[] { ' ', '\n' };
        string[] splitDescription = description.Split(separators, StringSplitOptions.RemoveEmptyEntries);
        foreach(var test in splitDescription)
        {
            finalDescription += test + ' ';
        }
        description = finalDescription;
    }

}
