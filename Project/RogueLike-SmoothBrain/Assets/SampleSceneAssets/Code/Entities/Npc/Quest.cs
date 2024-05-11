using DialogueSystem.Runtime;
using Map;
using Map.Generation;
using System;
using System.Collections;
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

    public QuestData Datas { get; private set; }
    public string progressText = string.Empty;
    static QuestDatabase database;
    public static event Action OnQuestUpdated;
    public static event Action OnQuestFinished;
    protected Hero player;
    protected QuestTalker.TalkerType talkerType;
    protected QuestTalker.TalkerGrade talkerGrade;
    protected QuestDifficulty difficulty;
    protected bool questLost = false;
    private int startFloor;

    private Coroutine timeManagerRoutine = null;
    protected float timeToFinishQuest;

    public float CurrentQuestTimer { get; protected set; }
    public int CorruptionModifierValue { get; protected set; } = 0;
    public QuestTalker.TalkerType TalkerType { get => talkerType; }
    public QuestDifficulty Difficulty { get => difficulty; }

    public abstract bool IsQuestFinished();
    protected abstract void ResetQuestValues();

    public virtual void AcceptQuest()
    {
        AudioManager.Instance.PlaySound(AudioManager.Instance.QuestObtainedSFX);
        MapUtilities.onEarlyAllEnemiesDead += CheckQuestFinished;
        MapUtilities.onEarlyAllChestOpen += CheckQuestFinished;
        MapUtilities.onEnter += CheckQuestFinished;
        Utilities.Hero.OnQuestObtained += CheckQuestFinished;

        startFloor = GameObject.FindAnyObjectByType<MapGenerator>().stage;
    }

    public void LateAcceptQuest()
    {
        if(Datas.LimitedTime)
        {
           timeManagerRoutine = CoroutineManager.Instance.StartCoroutine(TimeToFinishRoutine());
        }
    }

    protected void QuestFinished()
    {
        AudioManager.Instance.PlaySound(AudioManager.Instance.QuestFinishedSFX);
        player.CurrentQuest = null;

        if(startFloor == GameObject.FindAnyObjectByType<MapGenerator>().stage)
        {
            if (talkerGrade == QuestTalker.TalkerGrade.BOSS)
            {
                player.GetComponent<PlayerController>().DoneQuestQTThiStage = true;
            }
            else
            {
                player.GetComponent<PlayerController>().DoneQuestQTApprenticeThiStage = true;
            }
        }

        if(talkerType == QuestTalker.TalkerType.CLERIC)
        {
            player.Stats.DecreaseValue(Stat.CORRUPTION, CorruptionModifierValue);
        }
        else
        {
            player.Stats.IncreaseValue(Stat.CORRUPTION, CorruptionModifierValue);
        }
        

        Hero.CallCorruptionBenedictionText(talkerType == QuestTalker.TalkerType.CLERIC ? -CorruptionModifierValue : CorruptionModifierValue);
        OnQuestFinished?.Invoke();

        MapUtilities.onEarlyAllEnemiesDead -= CheckQuestFinished;
        MapUtilities.onEarlyAllChestOpen -= CheckQuestFinished;
        MapUtilities.onEnter -= CheckQuestFinished;
        Utilities.Hero.OnQuestObtained -= CheckQuestFinished;
        ResetQuestValues();

        if (timeManagerRoutine != null)
            CoroutineManager.Instance.StopCoroutine(timeManagerRoutine);
        timeManagerRoutine = null;

        HudHandler.current.MessageInfoHUD.Display($"You finished the quest <color=yellow>\"{Datas.idName.SeparateAllCase()}\"</color>.");
    }

    protected void QuestLost()
    {
        player.CurrentQuest = null;
        questLost = true;
        QuestUpdated();
        AudioManager.Instance.PlaySound(AudioManager.Instance.QuestLostSFX);
        MapUtilities.onEarlyAllEnemiesDead -= CheckQuestFinished;
        MapUtilities.onEarlyAllChestOpen -= CheckQuestFinished;
        MapUtilities.onEnter -= CheckQuestFinished;
        Utilities.Hero.OnQuestObtained -= CheckQuestFinished;
        ResetQuestValues();

        if (timeManagerRoutine != null)
            CoroutineManager.Instance.StopCoroutine(timeManagerRoutine);
        timeManagerRoutine = null;

        HudHandler.current.MessageInfoHUD.Display($"You lost the quest <color=yellow>\"{Datas.idName.SeparateAllCase()}\"</color>.");
    }

    protected void QuestUpdated()
    {
        if (IsQuestFinished())
        {
            HudHandler.current.QuestHUD.EmptyQuestTexts();
            HudHandler.current.QuestHUD.LostOrFinishedText.SetText("<color=yellow>Quest Completed!</color>\n Clear the current room to receive rewards!");
        }
        else if (questLost)
        {
            HudHandler.current.QuestHUD.EmptyQuestTexts();
            HudHandler.current.QuestHUD.LostOrFinishedText.SetText("<color=red>Quest Lost...</color>");
        }
        else
        {
            OnQuestUpdated?.Invoke();
        }
    }

    protected void CheckQuestFinished()
    {
        if (questLost)
        {
            QuestLost();
        }
        else if (IsQuestFinished())
        {
            QuestFinished();
        }
    }

    private IEnumerator TimeToFinishRoutine()
    {
        CurrentQuestTimer = timeToFinishQuest;
        while (CurrentQuestTimer > 0)
        {
            if (player == null || IsQuestFinished())
            {
                timeManagerRoutine = null;
                yield break;
            }

            CurrentQuestTimer -= Time.deltaTime;
            QuestUpdated();
            yield return null;
        }
        QuestLost();
        yield break;
    }

    #region STATIC_METHODS
    static public Quest LoadClass(string name, QuestDialogueDifficulty difficulty, QuestTalker questTalker)
    {
        if (database == null)
        {
            database = GameResources.Get<QuestDatabase>("QuestDatabase");
        }

        Quest quest = Assembly.GetExecutingAssembly().CreateInstance(name.GetPascalCase()) as Quest;
        quest.Datas = database.GetQuest(name);
        quest.player = GameObject.FindWithTag("Player").GetComponent<Hero>();
        quest.talkerType = questTalker.Type;
        quest.talkerGrade = questTalker.Grade;
        quest.CorruptionModifierValue = quest.Datas.CorruptionModifierValue;
        quest.difficulty = quest.Datas.HasDifferentGrades ? (QuestDifficulty)difficulty : QuestDifficulty.MEDIUM;
        InitDescription(ref quest.Datas.Description);
        return quest;
    }

    static public string GetRandomQuestName()
    {
        if (database == null)
        {
            database = GameResources.Get<QuestDatabase>("QuestDatabase");
        }

        int indexRandom = UnityEngine.Random.Range(0, database.datas.Count);
        return database.datas[indexRandom].idName;
    }

    static private void InitDescription(ref string description)
    {
        string finalDescription = string.Empty;
        char[] separators = new char[] { ' ', '\n' };
        string[] splitDescription = description.Split(separators, StringSplitOptions.RemoveEmptyEntries);
        foreach (var test in splitDescription)
        {
            finalDescription += test + ' ';
        }

        description = finalDescription;
    }
    #endregion
}
