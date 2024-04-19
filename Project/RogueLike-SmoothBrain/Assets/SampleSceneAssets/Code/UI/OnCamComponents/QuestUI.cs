using TMPro;
using UnityEngine;

public class QuestUI : MonoBehaviour
{
    private Hero player;
    [SerializeField] private TMP_Text description;
    [SerializeField] private TMP_Text title;
    [SerializeField] private TMP_Text progressText;


    [SerializeField] private RectTransform questTransform;
    private bool questEnable = false;
    private Coroutine questRoutine;

    void Start()
    {
        player = GameObject.FindWithTag("Player").GetComponent<Hero>();
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
            title.SetText(player.CurrentQuest.Datas.idName);
            description.SetText(player.CurrentQuest.Datas.Description);
            progressText.SetText(player.CurrentQuest.progressText);
        }
    }
}
