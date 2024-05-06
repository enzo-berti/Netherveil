using DialogueSystem.Runtime;
using Map.Generation;
using UnityEngine;

public class QuestTalker : Npc
{
    [Header("Talker parameters")]
    [SerializeField] protected DialogueTree questDT;
    [SerializeField] protected DialogueTree refusesDialogueDT;
    [SerializeField] protected DialogueTree alreadyHaveQuestDT;
    [SerializeField] protected DialogueTree alreadyDoneQuestDT;
    protected DialogueTreeRunner dialogueTreeRunner;
    protected Hero player;
    static QuestDatabase database;
    public enum TalkerType
    {
        CLERIC,
        SHAMAN
    }

    public enum TalkerGrade
    {
        BOSS,
        APPRENTICE
    }

    [SerializeField] protected TalkerType type;
    [SerializeField] protected TalkerGrade grade;
    public TalkerType Type => type;
    public TalkerGrade Grade => grade;
    public Quest.QuestDifficulty QuestDifficulty { get; private set; }
    public int QuestIndex { get; private set; }

    protected override void Awake()
    {
        if (database == null)
        {
            database = GameResources.Get<QuestDatabase>("QuestDatabase");
        }

        QuestIndex = Seed.Range(0, database.datas.Count);

        QuestDifficulty = database.GetQuest(database.datas[QuestIndex].idName).HasDifferentGrades ?
            (Quest.QuestDifficulty)Seed.Range(0, (int)Quest.QuestDifficulty.NB) : Quest.QuestDifficulty.MEDIUM;
    }

    protected override void Start()
    {
        base.Start();
        dialogueTreeRunner = FindObjectOfType<DialogueTreeRunner>();
        player = GameObject.FindWithTag("Player").GetComponent<Hero>();
    }

    public override void Interract()
    {
        if (dialogueTreeRunner.IsStarted)
        {
            dialogueTreeRunner.UpdateDialogue();
        }
        else
        {
            StartDialogue();
        }
    }

    protected virtual void StartDialogue()
    {
        DialogueTree dialogue = questDT;

        if (PlayerInvestedInOppositeWay())
        {
            dialogue = refusesDialogueDT;
        }
        else if (player.CurrentQuest != null)
        {
            dialogue = alreadyHaveQuestDT;
        }
        else if (player.GetComponent<PlayerController>().DoneQuestQTThiStage)
        {
            dialogue = alreadyDoneQuestDT;
        }

        dialogueTreeRunner.NameBackgroundImage.color = type == TalkerType.CLERIC ? Hero.benedictionColor2 : Hero.corruptionColor2;
        dialogueTreeRunner.StartDialogue(dialogue, this);
    }

    protected bool PlayerInvestedInOppositeWay()
    {
        if (type == TalkerType.CLERIC && player.Stats.GetValue(Stat.CORRUPTION) >= player.STEP_VALUE)
        {
            return true;
        }
        else if (type == TalkerType.SHAMAN && player.Stats.GetValue(Stat.CORRUPTION) <= -player.STEP_VALUE)
        {
            return true;
        }
        return false;
    }

    public string GetQuestName()
    {
        return database.datas[QuestIndex].idName;
    }
}
