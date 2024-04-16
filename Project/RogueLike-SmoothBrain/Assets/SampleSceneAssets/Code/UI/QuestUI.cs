using TMPro;
using UnityEngine;

public class QuestUI : MonoBehaviour
{
    Hero player;
    [SerializeField] TMP_Text description;
    [SerializeField] TMP_Text title;
    [SerializeField] TMP_Text progressText;

    void Start()
    {
        player = GameObject.FindWithTag("Player").GetComponent<Hero>();
        Hero.OnQuestObtained += UpdateUI;
        Hero.OnQuestFinished += UpdateUI;
        Quest.OnQuestUpdated += UpdateUI;
    }

    private void OnDestroy()
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
