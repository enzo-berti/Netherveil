using DialogueSystem.Runtime;
using System.Collections;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    [Header("GameObjects & Components")]
    [SerializeField] private Transform choicesTransform;
    [SerializeField] private TMP_Text nameMesh;
    [SerializeField] private TMP_Text dialogueMesh;
    [SerializeField] private Image illustrationImage;
    [SerializeField] private Button choicePrefab;
    private Animator animator;

    [Space, Header("Parameters")]
    [SerializeField] private float timeBetweenLetters = 0.025f;
    private DialogueContainer currentDialogue;
    private string curNarrativeDataGUID;
    private Coroutine coroutine;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void StartDialogue(DialogueContainer dialogue)
    {
        currentDialogue = dialogue;
        var narrativeData = currentDialogue.nodeLinks.First(); //Entrypoint node
        ProceedToNarrative(narrativeData.TargetNodeGuid);
        animator.SetBool("ActiveDialogue", true);
    }

    private void ProceedToNarrative(string narrativeDataGUID)
    {
        curNarrativeDataGUID = narrativeDataGUID;
        DialogueNodeData curData = currentDialogue.dialogueNodeData.Find(x => x.Guid == curNarrativeDataGUID);

        ClearChoices();

        nameMesh.gameObject.SetActive(!string.IsNullOrEmpty(curData.nameText));
        if (!string.IsNullOrEmpty(curData.nameText))
        {
            nameMesh.text = curData.nameText;
        }

        dialogueMesh.gameObject.SetActive(!string.IsNullOrEmpty(curData.dialogueText));
        if (!string.IsNullOrEmpty(curData.dialogueText))
        {
            if (coroutine != null)
                StopCoroutine(coroutine);

            coroutine = StartCoroutine(TypeSentence(curData.dialogueText));
        }

        illustrationImage.gameObject.SetActive(curData.illustrationSprite != null);
        if (curData.illustrationSprite != null)
        {
            illustrationImage.sprite = curData.illustrationSprite;
        }
    }

    private IEnumerator TypeSentence(string sentence)
    {
        dialogueMesh.text = "";
        foreach (char letter in sentence.ToCharArray())
        {
            dialogueMesh.text += letter;
            yield return new WaitForSeconds(timeBetweenLetters);
        }
        OnFinishTyping();
    }

    public void ClearChoices()
    {
        var buttons = choicesTransform.GetComponentsInChildren<Button>();
        for (int i = 0; i < buttons.Length; i++)
        {
            Destroy(buttons[i].gameObject);
        }
    }

    public void OnFinishTyping()
    {
        DialogueNodeData curData = currentDialogue.dialogueNodeData.Find(x => x.Guid == curNarrativeDataGUID);
        dialogueMesh.text = curData.dialogueText;

        if (curData.type == "Choice" && coroutine != null)
        {
            var choices = currentDialogue.nodeLinks.Where(x => x.BaseNodeGuid == curNarrativeDataGUID);
            foreach (var choice in choices)
            {
                var button = Instantiate(choicePrefab, choicesTransform);
                button.gameObject.SetActive(true);
                button.GetComponentInChildren<TMP_Text>().text = choice.PortName;
                button.onClick.AddListener(() => ProceedToNarrative(choice.TargetNodeGuid));
            }
        }
        else if (curData.type == "Text" && coroutine == null)
        {
            var next = currentDialogue.nodeLinks.Where(x => x.BaseNodeGuid == curNarrativeDataGUID).FirstOrDefault();
            ProceedToNarrative(next.TargetNodeGuid);
        }

        coroutine = null;
    }

    public void CheckFinish()
    {
        if (!currentDialogue.nodeLinks.Any(x => x.BaseNodeGuid == curNarrativeDataGUID) && coroutine == null)
        {
            animator.SetBool("ActiveDialogue", false);
        }
    }
}
