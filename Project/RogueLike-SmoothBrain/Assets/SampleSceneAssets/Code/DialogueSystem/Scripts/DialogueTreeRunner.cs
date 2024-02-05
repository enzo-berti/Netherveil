using DialogueSystem.Runtime;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueTreeRunner : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Canvas dialogueCanvas;
    [SerializeField] private Image illustrationImage;
    [SerializeField] private TMP_Text nameMesh;
    [SerializeField] private TMP_Text dialogueMesh;
    [SerializeField] private Transform choiceTab;

    [Header("Parameters")]
    private DialogueTree tree;
    [SerializeField, Range(0f, 1f)] private float letterDelay = 0.1f;
    [SerializeField] private Button choiceButtonPrefab;
    private bool isStarted => dialogueCanvas.gameObject.activeSelf;
    private string lastDialogue;

    private void Update()
    {
        if (isStarted && Input.GetKeyDown(KeyCode.E) && tree != null)
        {
            UpdateDialogue();
        }
    }

    public void StartDialogue(DialogueTree tree)
    {
        if (isStarted)
            return;

        this.tree = tree;
        this.tree.ResetTree();
        dialogueCanvas.gameObject.SetActive(true);
    }

    public void EndDialogue()
    {
        dialogueCanvas.gameObject.SetActive(false);
    }

    private void UpdateDialogue()
    {
        foreach (Transform child in choiceTab)
            Destroy(child.gameObject);

        SimpleDialogueNode simple = tree.currentNode as SimpleDialogueNode;
        ChoiceDialogueNode choice = tree.currentNode as ChoiceDialogueNode;

        if (simple)
        {
            SetIllustration(simple.dialogueData.illustration);
            SetName(simple.dialogueData.name);
            SetDialogue(simple.dialogueData.dialogue);

            tree.Process(simple.child);
        }
        else if (choice)
        {
            SetIllustration(choice.dialogueData.illustration);
            SetName(choice.dialogueData.name);
            SetDialogue(choice.dialogueData.dialogue);

            choice.options.ForEach(choiceData =>
            {
                Button newChoiceButton = Instantiate(choiceButtonPrefab, choiceTab);
                newChoiceButton.transform.GetComponentInChildren<TMP_Text>().text = choiceData.option;
                newChoiceButton.onClick.AddListener(() =>
                {
                    tree.Process(choiceData.child);
                    UpdateDialogue();
                });
            });
        }
        else if (tree.currentNode == null)
        {
            EndDialogue();
        }
    }

    private void SetIllustration(Sprite illustration)
    {
        if (!illustration)
        {
            illustrationImage.gameObject.SetActive(false);
            return;
        }

        illustrationImage.gameObject.SetActive(true);
        illustrationImage.sprite = illustration;
    }

    private void SetName(string name)
    {
        if (string.IsNullOrEmpty(name))
        {
            nameMesh.gameObject.SetActive(false);
            return;
        }

        nameMesh.gameObject.SetActive(true);
        nameMesh.text = name;
    }

    private void SetDialogue(string dialogue)
    {
        if (string.IsNullOrEmpty(dialogue))
        {
            dialogueMesh.gameObject.SetActive(false);
            return;
        }
        dialogueMesh.gameObject.SetActive(true);

        lastDialogue = dialogue;
        StopAllCoroutines();
        StartCoroutine(WriteDialogue(dialogue));
    }

    private IEnumerator WriteDialogue(string dialogue)
    {
        dialogueMesh.text = string.Empty;
        foreach (var letter in dialogue)
        {
            yield return new WaitForSecondsRealtime(letterDelay);
            dialogueMesh.text += letter;
        }
        dialogueMesh.text = dialogue;
    }
}
