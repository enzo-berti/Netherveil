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


    [SerializeField] private RectTransform questTransform;
    private bool questEnable = false;
    public bool QuestEnable { get => questEnable; }
    private Coroutine questRoutine;

    void Start()
    {
        player = GameObject.FindWithTag("Player").GetComponent<Hero>();

        title.SetText(string.Empty);
        description.SetText(string.Empty);
        rewardText.SetText(string.Empty);
        progressText.SetText(string.Empty);
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
        Hero.OnQuestObtained += UpdateUI;
        Hero.OnQuestFinished += UpdateUI;
        Quest.OnQuestUpdated += UpdateUI;
    }

    private void OnDisable()
    {
        Hero.OnQuestObtained -= UpdateUI;
        Hero.OnQuestFinished -= UpdateUI;
        Quest.OnQuestUpdated -= UpdateUI;
    }

    private void UpdateUI()
    {
        bool hasQuest = player.CurrentQuest != null;
        foreach (Transform children in transform)
        {
            children.gameObject.SetActive(hasQuest);
        }

        if(hasQuest)
        {
            string rewardName = player.CurrentQuest.TalkerType == QuestTalker.TalkerType.SHAMAN ? "<color=purple>corruption</color>" : "<color=yellow>benediction</color>";
            int absValue = Mathf.Abs(player.CurrentQuest.CorruptionModifierValue);

            title.SetText(player.CurrentQuest.Datas.idName);
            description.SetText(player.CurrentQuest.Datas.Description);
            rewardText.SetText($"\nRewards: {absValue} {rewardName}");
            progressText.SetText(player.CurrentQuest.progressText);

            description.GetComponent<ContentSizeFitter>().SetLayoutVertical();
        }
    }
}
