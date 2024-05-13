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

        lostOrFinishedText.SetText("No Quests...");
        EmptyQuestTexts();

        Utilities.Hero.OnQuestObtained += UpdateUI;
        Utilities.Hero.OnQuestFinished += UpdateUI;
    }

    public void EmptyQuestTexts()
    {
        title.SetText(string.Empty);
        description.SetText(string.Empty);
        rewardText.SetText(string.Empty);
        progressText.SetText(string.Empty);
        difficultyText.SetText(string.Empty);
    }

    public void Toggle()
    {
        if (!gameObject.activeInHierarchy)
            return;

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
        Quest.OnQuestUpdated += UpdateUI;
    }

    private void OnDisable()
    {
        if (questRoutine != null)
        {
            StopCoroutine(questRoutine);

            Vector3 anchoredPos = questTransform.anchoredPosition;
            anchoredPos.x = questTransform.sizeDelta.x;
            questTransform.anchoredPosition = anchoredPos;

            questEnable = false;
        }

        Quest.OnQuestUpdated -= UpdateUI;
    }

    private void UpdateUI()
    {
        bool hasQuest = player.CurrentQuest != null;

        if (hasQuest)
        {
            lostOrFinishedText.SetText(string.Empty);

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
                        difficultyText.SetText("<color=orange>Medium</color>");
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
            string titleText = player.CurrentQuest.TalkerType == QuestTalker.TalkerType.SHAMAN ?
                $"<color=purple>{player.CurrentQuest.Datas.idName}</color>" :
                $"<color=yellow>{player.CurrentQuest.Datas.idName}</color>";

            title.SetText(titleText);
            description.SetText(player.CurrentQuest.Datas.Description);
            rewardText.SetText($"\n <color=yellow>Reward : </color> {absValue} {rewardName}");
            progressText.SetText(player.CurrentQuest.progressText + "\n" + GetTimeString());

            description.GetComponent<ContentSizeFitter>().SetLayoutVertical();
        }
        else
        {
            EmptyQuestTexts();
        }
    }

    private string GetTimeString()
    {
        if(!player.CurrentQuest.Datas.LimitedTime || player.CurrentQuest.IsQuestFinished())
            return string.Empty;

        if(player.CurrentQuest.CurrentQuestTimer < 60)
            return "<color=red>" + Math.Round(player.CurrentQuest.CurrentQuestTimer, 1) + " seconds remaining</color>";
        else
            return Math.Round(player.CurrentQuest.CurrentQuestTimer, 1) + " seconds remaining";
    }
}
