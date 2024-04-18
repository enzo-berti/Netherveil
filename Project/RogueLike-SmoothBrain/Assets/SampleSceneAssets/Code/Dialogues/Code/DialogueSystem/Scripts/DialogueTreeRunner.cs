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
    public bool IsStarted => dialogueCanvas.gameObject.activeSelf;
    private string lastDialogue;
    DialogueTreeEventManager eventManager;
    public DialogueTreeEventManager EventManager { get => eventManager; }
    public QuestTalker TalkerNPC { get; private set; }
    private Hero player;

    private void Awake()
    {
        eventManager = GetComponent<DialogueTreeEventManager>();
        player = GameObject.FindWithTag("Player").GetComponent<Hero>();
    }

    public void StartDialogue(DialogueTree tree, Npc npc)
    {
        if (IsStarted)
            return;

        this.tree = tree;
        this.tree.ResetTree();

        dialogueCanvas.gameObject.SetActive(true);

        if (npc is QuestTalker)
            TalkerNPC = npc as QuestTalker;

        UpdateDialogue();
    }

    public void EndDialogue()
    {
        dialogueCanvas.gameObject.SetActive(false);
        TalkerNPC = null;
    }

    public void UpdateDialogue()
    {
        if(!IsStarted || tree == null)
            return;

        foreach (Transform child in choiceTab)
            Destroy(child.gameObject);

        SimpleDialogueNode simple = tree.currentNode as SimpleDialogueNode;
        ChoiceDialogueNode choice = tree.currentNode as ChoiceDialogueNode;
        EventDialogueNode eventN = tree.currentNode as EventDialogueNode;
        QuestDialogueNode quest = tree.currentNode as QuestDialogueNode;

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
        else if (eventN)
        {
            SetIllustration(eventN.dialogueData.illustration);
            SetName(eventN.dialogueData.name);
            SetDialogue(eventN.dialogueData.dialogue);

            tree.Process(eventN.child);

            eventManager.Invoke(eventN.eventTag);
        }
        else if (quest)
        {
            SetIllustration(quest.dialogueData.illustration);
            SetName(quest.dialogueData.name);
            SetDialogue(quest.dialogueData.dialogue);

            tree.Process(quest.child);

            if (TalkerNPC != null)
            {
                if (string.IsNullOrEmpty(quest.questTag))
                    player.CurrentQuest = Quest.LoadClass(Quest.GetRandomQuestName(), TalkerNPC.Type, TalkerNPC.Grade);
                else
                    player.CurrentQuest = Quest.LoadClass(quest.questTag, TalkerNPC.Type, TalkerNPC.Grade);
            }
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
