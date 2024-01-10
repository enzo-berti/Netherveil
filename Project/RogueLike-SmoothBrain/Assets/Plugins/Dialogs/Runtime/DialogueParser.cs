using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DialogueSystem.Runtime;

public class DialogueParser : MonoBehaviour
{
    [SerializeField] private DialogueContainer dialogue;
    [SerializeField] private TMP_Text dialogueText;
    [SerializeField] private Button choicePrefab;
    [SerializeField] private Transform buttonContainer;

    public void Play()
    {
        var narrativeData = dialogue.nodeLinks.First(); //Entrypoint node
        ProceedToNarrative(narrativeData.TargetNodeGuid);
    }

    private void ProceedToNarrative(string narrativeDataGUID)
    {
        var text = dialogue.dialogueNodeData.Find(x => x.Guid == narrativeDataGUID).dialogueText;
        var choices = dialogue.nodeLinks.Where(x => x.BaseNodeGuid == narrativeDataGUID);
        dialogueText.text = ProcessProperties(text);
        var buttons = buttonContainer.GetComponentsInChildren<Button>();
        for (int i = 0; i < buttons.Length; i++)
        {
            Destroy(buttons[i].gameObject);
        }

        foreach (var choice in choices)
        {
            var button = Instantiate(choicePrefab, buttonContainer);
            button.gameObject.SetActive(true);
            button.GetComponentInChildren<TMP_Text>().text = ProcessProperties(choice.PortName);
            button.onClick.AddListener(() => ProceedToNarrative(choice.TargetNodeGuid));
        }
    }

    private string ProcessProperties(string text)
    {
        foreach (var exposedProperty in dialogue.exposedProperties)
        {
            text = text.Replace($"[{exposedProperty.propertyName}]", exposedProperty.propertyValue);
        }
        return text;
    }
}
