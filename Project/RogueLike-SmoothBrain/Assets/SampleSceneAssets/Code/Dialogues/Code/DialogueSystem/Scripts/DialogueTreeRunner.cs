using DialogueSystem.Runtime;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
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
    private bool isRunning = false;
    private bool isLaunched = false;

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
        isLaunched = false;
        isRunning = false;
    }

    public void UpdateDialogue()
    {
        if (!IsStarted || tree == null)
            return;

        foreach (Transform child in choiceTab)
            Destroy(child.gameObject);

        SimpleDialogueNode simple = tree.currentNode as SimpleDialogueNode;
        ChoiceDialogueNode choice = tree.currentNode as ChoiceDialogueNode;
        EventDialogueNode eventN = tree.currentNode as EventDialogueNode;
        QuestDialogueNode quest = tree.currentNode as QuestDialogueNode;

        if (simple)
        {
            if (isRunning)
            {
                StopAllCoroutines();
                dialogueMesh.text = simple.dialogueData.dialogue;
                isRunning = false;
            }
            else if (!isLaunched)
            {
                SetDialogue(simple.dialogueData.dialogue);
                SetIllustration(simple.dialogueData.illustration);
                SetName(simple.dialogueData.name);
            }
            else if (isLaunched && !isRunning)
            {
                tree.Process(simple.child);
                isLaunched = false;
                UpdateDialogue();
            }
        }
        else if (choice)
        {
            if (isRunning)
            {
                StopAllCoroutines();
                dialogueMesh.text = choice.dialogueData.dialogue;
                isRunning = false;
            }
            else if (!isLaunched)
            {
                SetDialogue(choice.dialogueData.dialogue);
                SetIllustration(choice.dialogueData.illustration);
                SetName(choice.dialogueData.name);
            }

            choice.options.ForEach(choiceData =>
            {
                Button newChoiceButton = Instantiate(choiceButtonPrefab, choiceTab);
                newChoiceButton.transform.GetComponentInChildren<TMP_Text>().text = choiceData.option;
                newChoiceButton.onClick.AddListener(() =>
                {
                    StopAllCoroutines();
                    tree.Process(choiceData.child);
                    isLaunched = false;
                    isRunning = false;
                    UpdateDialogue();
                });

                EventSystem.current.SetSelectedGameObject(newChoiceButton.gameObject);
            });
        }
        else if (eventN)
        {
            if (isRunning)
            {
                StopAllCoroutines();
                dialogueMesh.text = eventN.dialogueData.dialogue;
                isRunning = false;
            }
            else if (!isLaunched)
            {
                SetDialogue(eventN.dialogueData.dialogue);
                SetIllustration(eventN.dialogueData.illustration);
                SetName(eventN.dialogueData.name);
            }
            else if (isLaunched && !isRunning)
            {
                eventManager.Invoke(eventN.eventTag);
                tree.Process(eventN.child);
                isLaunched = false;
                UpdateDialogue();
            }
        }
        else if (quest)
        {
            if (isRunning)
            {
                StopAllCoroutines();
                dialogueMesh.text = eventN.dialogueData.dialogue;
                isRunning = false;
            }
            else if (!isLaunched)
            {
                SetDialogue(eventN.dialogueData.dialogue);
                SetIllustration(eventN.dialogueData.illustration);
                SetName(eventN.dialogueData.name);
            }
            else if (isLaunched && !isRunning)
            {
                if (TalkerNPC != null)
                {
                    if (string.IsNullOrEmpty(quest.questTag))
                        player.CurrentQuest = Quest.LoadClass(Quest.GetRandomQuestName(), TalkerNPC.Type, TalkerNPC.Grade);
                    else
                        player.CurrentQuest = Quest.LoadClass(quest.questTag, TalkerNPC.Type, TalkerNPC.Grade);
                }

                tree.Process(quest.child);
                isLaunched = false;
                UpdateDialogue();
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
        isRunning = true;
        isLaunched = true;
        foreach (var letter in dialogue)
        {
            yield return new WaitForSeconds(letterDelay);
            dialogueMesh.text += letter;
        }
        isRunning = false;
        dialogueMesh.text = dialogue;
    }
}
