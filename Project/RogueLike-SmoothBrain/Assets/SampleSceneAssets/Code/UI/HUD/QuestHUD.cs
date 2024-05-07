using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuestHUD : MonoBehaviour
{
    private Hero player;
    [SerializeField] private TMP_Text description;
    [SerializeField] private TMP_Text title;
    [SerializeField] private TMP_Text rewardText;
    [SerializeField] private TMP_Text progressText;
    [SerializeField] private TMP_Text difficultyText;
    [SerializeField] private TMP_Text lostOrFinishedText;


    [SerializeField] private RectTransform questTransform;
    private bool questEnable = false;
    public bool QuestEnable { get => questEnable; }
    public TMP_Text LostOrFinishedText { get => lostOrFinishedText; }
    private Coroutine questRoutine;

    void Start()
    {
        player = GameObject.FindWithTag("Player").GetComponent<Hero>();

        title.SetText("No Quests...");
        description.SetText(string.Empty);
        rewardText.SetText(string.Empty);
        progressText.SetText(string.Empty);
        difficultyText.SetText(string.Empty);
    }

    public void Toggle()
    {
        if (!questEnable)
        {
            if (questRoutine != null)
                StopCoroutine(questRoutine);

            questTransform.anchoredPosition = new Vector2(questTransform.sizeDelta.x, questTransform.anchoredPosition.y);
            questRoutine = StartCoroutine(questTransform.TranslateX(0.1f, -questTransform.sizeDelta.x));
        }
        else
        {
            if (questRoutine != null)
                StopCoroutine(questRoutine);

            questTransform.anchoredPosition = new Vector2(0.0f, questTransform.anchoredPosition.y);
            questRoutine = StartCoroutine(questTransform.TranslateX(0.1f, questTransform.sizeDelta.x));
        }
        questEnable = !questEnable;
    }

    private void OnEnable()
    {
        Utilities.Hero.OnQuestObtained += UpdateUI;
        Utilities.Hero.OnQuestFinished += UpdateUI;
        Quest.OnQuestUpdated += UpdateUI;
    }

    private void OnDisable()
    {
        Quest.OnQuestUpdated -= UpdateUI;
    }

    private void UpdateUI()
    {
        bool hasQuest = player.CurrentQuest != null;

        if (hasQuest)
        {
            lostOrFinishedText.SetText(string.Empty);
            float initialSize = rewardText.fontSize;

            string rewardName = player.CurrentQuest.TalkerType == QuestTalker.TalkerType.SHAMAN ? 
                $"<sprite name=\"corruption\">" : 
                $"<sprite name=\"benediction\">";

            int absValue = Mathf.Abs(player.CurrentQuest.CorruptionModifierValue);

            if (player.CurrentQuest.Datas.HasDifferentGrades)
            {
                switch (player.CurrentQuest.Difficulty)
                {
                    case Quest.QuestDifficulty.EASY:
                        difficultyText.SetText("<color=green>Easy</color>");
                        break;
                    case Quest.QuestDifficulty.MEDIUM:
                        difficultyText.SetText("<color=yellow>Medium</color>");
                        break;
                    case Quest.QuestDifficulty.HARD:
                        difficultyText.SetText("<color=red>Hard</color>");
                        break;
                    default:
                        difficultyText.SetText("ERROR");
                        break;
                }
            }
            else
            {
                difficultyText.SetText(string.Empty);
            }

            //
            title.SetText(player.CurrentQuest.Datas.idName);
            description.SetText(player.CurrentQuest.Datas.Description);
            rewardText.SetText($"\nReward: {absValue} {rewardName}");
            progressText.SetText(player.CurrentQuest.progressText + "\n" + GetTimeString());

            description.GetComponent<ContentSizeFitter>().SetLayoutVertical();
        }
        else
        {
            title.SetText(string.Empty);
            description.SetText(string.Empty);
            rewardText.SetText(string.Empty);
            progressText.SetText(string.Empty);
            difficultyText.SetText(string.Empty);
        }
    }

    private string GetTimeString()
    {
        if(!player.CurrentQuest.Datas.LimitedTime)
            return string.Empty;

        if(player.CurrentQuest.CurrentQuestTimer < 60)
            return "<color=red>" + Math.Round(player.CurrentQuest.CurrentQuestTimer, 1) + " seconds remaining</color>";
        else
            return Math.Round(player.CurrentQuest.CurrentQuestTimer, 1) + " seconds remaining";
    }
}
