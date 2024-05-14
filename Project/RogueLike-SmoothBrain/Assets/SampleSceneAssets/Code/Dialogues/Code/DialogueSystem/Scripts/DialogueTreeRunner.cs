using DialogueSystem.Runtime;
using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.DualShock;
using UnityEngine.InputSystem.Samples.RebindUI;
using UnityEngine.UI;

public class DialogueTreeRunner : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Canvas dialogueCanvas;
    [SerializeField] private Image illustrationImage;
    [SerializeField] private TMP_Text nameMesh;
    [SerializeField] private TMP_Text dialogueMesh;
    [SerializeField] private Transform choiceTab;

    [SerializeField] private KeybindingsIcons iconsList;
    [SerializeField] private InputActionReference skipKB;
    [SerializeField] private InputActionReference skipGamepad;
    [SerializeField] private TMP_Text skipDialogueText;
    public Image NameBackgroundImage;

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
    private bool hasRenderedChoices = false;

    public bool IsRunning => isRunning;

    private float iconSize;
    private float normalSize;

    private void Awake()
    {
        eventManager = GetComponent<DialogueTreeEventManager>();
        player = GameObject.FindWithTag("Player").GetComponent<Hero>();
        normalSize = skipDialogueText.fontSize;
        iconSize = skipDialogueText.fontSize + 10;
        UpdateBinding();
        DeviceManager.OnChangedToKB += UpdateBinding;
        DeviceManager.OnChangedToGamepad += UpdateBinding;
        PauseMenu.OnUnpause += UpdateBinding;
    }

    private void OnDestroy()
    {
        DeviceManager.OnChangedToKB -= UpdateBinding;
        DeviceManager.OnChangedToGamepad -= UpdateBinding;
        PauseMenu.OnUnpause -= UpdateBinding;
    }

    private void UpdateBinding()
    {
        if (DeviceManager.Instance.IsPlayingKB())
        {
            skipDialogueText.text = $"Skip <size={iconSize}><sprite name=\"{GetDisplayString(skipKB).GetCamelCase()}\"><size={normalSize}>";
        }
        else
        {
            skipDialogueText.text = $"Skip <size={iconSize}><sprite name=\"{GetDisplayString(skipGamepad).GetCamelCase()}\"><size={normalSize}>";
        }
    }

    public void StartDialogue(DialogueTree tree, Npc npc)
    {
        if (IsStarted)
            return;

        HudHandler.current.SetActive(false, 0.25f);
        this.tree = tree;
        this.tree.ResetTree();
        Utilities.PlayerInput.DisableGameplayInputs();

        dialogueCanvas.gameObject.SetActive(true);

        if (npc is QuestTalker)
            TalkerNPC = npc as QuestTalker;

        UpdateDialogue();
    }

    public void EndDialogue()
    {
        dialogueCanvas.gameObject.SetActive(false);
        Utilities.PlayerInput.EnableGameplayInputs();
        HudHandler.current.SetActive(true, 0.25f);
        TalkerNPC = null;
        isLaunched = false;
        isRunning = false;
    }

    public bool IsCurrentDialogueChoiceDialogue()
    {
        if (tree == null || tree.currentNode == null)
            return false;

        return tree.currentNode is ChoiceDialogueNode;
    }

    public bool IsNextDialogueChoiceDialogue()
    {
        if (tree == null || tree.currentNode == null)
            return false;

        SimpleDialogueNode simple = tree.currentNode as SimpleDialogueNode;
        ChoiceDialogueNode choice = tree.currentNode as ChoiceDialogueNode;
        EventDialogueNode eventN = tree.currentNode as EventDialogueNode;
        QuestDialogueNode quest = tree.currentNode as QuestDialogueNode;

        if (simple && simple.child != null && simple.child is ChoiceDialogueNode) return true;
        if (eventN && eventN.child != null && eventN.child is ChoiceDialogueNode) return true;
        if (quest && quest.child != null && quest.child is ChoiceDialogueNode) return true;

        if (choice)
        {
            foreach (ChoiceDialogueNode.Option option in choice.options)
            {
                if (option.child != null && option.child is ChoiceDialogueNode) return true;
            }
        }

        return false;
    }

    public void UpdateDialogue()
    {
        if (!IsStarted || tree == null || hasRenderedChoices)
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
                StartCoroutine(ChoiceButton(choice));
                hasRenderedChoices = true;
            }
            else if (!isLaunched)
            {
                SetDialogue(choice.dialogueData.dialogue);
                SetIllustration(choice.dialogueData.illustration);
                SetName(choice.dialogueData.name);
            }
            else if (isLaunched && !isRunning && !hasRenderedChoices)
            {
                StartCoroutine(ChoiceButton(choice));
                hasRenderedChoices = true;
            }
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
                dialogueMesh.text = quest.dialogueData.dialogue;
                isRunning = false;
            }
            else if (!isLaunched)
            {
                SetDialogue(quest.dialogueData.dialogue);
                SetIllustration(quest.dialogueData.illustration);
                SetName(quest.dialogueData.name);
            }
            else if (isLaunched && !isRunning)
            {
                HudHandler.current.SetActive(true, 0.25f);
                if (TalkerNPC != null)
                {
                    if (string.IsNullOrEmpty(quest.questTag))
                        player.CurrentQuest = Quest.LoadClass(TalkerNPC.GetQuestName(), quest.difficulty, TalkerNPC);
                    else
                        player.CurrentQuest = Quest.LoadClass(quest.questTag, quest.difficulty, TalkerNPC);
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

    private IEnumerator ChoiceButton(ChoiceDialogueNode choice)
    {
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
                hasRenderedChoices = false;
                UpdateDialogue();
            });
        });
        yield return new WaitForSeconds(0.1f);
        EventSystem.current.SetSelectedGameObject(choiceTab.GetChild(0).gameObject);
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
        if (tree.currentNode is ChoiceDialogueNode)
        {
            UpdateDialogue();
        }
    }

    private string GetDisplayString(InputActionReference actionRef, int bindingIndex = 0)
    {
        // Get display string from action.
        var action = actionRef != null ? actionRef.action : null;
        string displayString = string.Empty;

        if (action != null)
        {

            displayString = GetAppropriateKeyString(actionRef, bindingIndex);
            if (!DeviceManager.Instance.IsPlayingKB() && DeviceManager.Instance.CurrentDevice is DualShockGamepad)
            {
                displayString += "_ps";
            }
            else if (!DeviceManager.Instance.IsPlayingKB())
            {
                displayString += "_xbox";
            }
        }

        return displayString;
    }

    private string GetAppropriateKeyString(InputActionReference actionRef, int bindingIndex = 0)
    {
        var action = actionRef != null ? actionRef.action : null;
        string bindingDisplayString = action.GetBindingDisplayString(bindingIndex, out _, out string controlPath, InputBinding.DisplayStringOptions.DontUseShortDisplayNames);

        //OEM represents keys specific to specific keyboard layout or even manufacturers so there are too many different things it can be so
        //we just ignore it display a placeholder icon key
        if (controlPath.Contains("OEM"))
            return controlPath;

        switch (controlPath)
        {
            //keyboard bindings
            case "escape": return controlPath;
            case "space": return controlPath;
            case "enter": return controlPath;
            case "tab": return controlPath;
            case "backquote": return controlPath;
            //case "quote": return controlPath;
            //case "semicolon": return controlPath;
            //case "comma": return controlPath;
            //case "period": return controlPath;
            //case "slash": return controlPath;
            //case "backslash": return controlPath;
            //case "leftBracket": return controlPath;
            //case "rightBracket": return controlPath;
            //case "minus": return controlPath;
            case "equals": return controlPath;
            case "upArrow": return controlPath;
            case "downArrow": return controlPath;
            case "leftArrow": return controlPath;
            case "rightArrow": return controlPath;
            case "1": return controlPath;
            case "2": return controlPath;
            case "3": return controlPath;
            case "4": return controlPath;
            case "5": return controlPath;
            case "6": return controlPath;
            case "7": return controlPath;
            case "8": return controlPath;
            case "9": return controlPath;
            case "0": return controlPath;
            case "leftShift": return controlPath;
            case "rightShift": return controlPath;
            case "shift": return controlPath;
            case "leftAlt": return controlPath;
            case "rightAlt": return controlPath;
            case "alt": return controlPath;
            case "leftCtrl": return controlPath;
            case "rightCtrl": return controlPath;
            case "ctrl": return controlPath;
            case "leftMeta": return controlPath;
            case "rightMeta": return controlPath;
            case "contextMenu": return controlPath;
            case "backspace": return controlPath;
            case "pageDown": return controlPath;
            case "pageUp": return controlPath;
            case "home": return controlPath;
            case "end": return controlPath;
            case "insert": return controlPath;
            case "delete": return controlPath;
            case "capsLock": return controlPath;
            case "numLock": return controlPath;
            case "printScreen": return controlPath;
            case "scrollLock": return controlPath;
            case "pause": return controlPath;
            case "numpadEnter": return controlPath;
            case "numpadDivide": return controlPath;
            case "numpadMultiply": return controlPath;
            case "numpadPlus": return controlPath;
            case "numpadMinus": return controlPath;
            case "numpadPeriod": return controlPath;
            case "numpadEquals": return controlPath;
            case "numpad1": return controlPath;
            case "numpad2": return controlPath;
            case "numpad3": return controlPath;
            case "numpad4": return controlPath;
            case "numpad5": return controlPath;
            case "numpad6": return controlPath;
            case "numpad7": return controlPath;
            case "numpad8": return controlPath;
            case "numpad9": return controlPath;
            case "numpad0": return controlPath;
            case "f1": return controlPath;
            case "f2": return controlPath;
            case "f3": return controlPath;
            case "f4": return controlPath;
            case "f5": return controlPath;
            case "f6": return controlPath;
            case "f7": return controlPath;
            case "f8": return controlPath;
            case "f9": return controlPath;
            case "f10": return controlPath;
            case "f11": return controlPath;
            case "f12": return controlPath;
            //mouse bindings
            case "leftButton": return controlPath;
            case "rightButton": return controlPath;
            case "middleButton": return controlPath;
            case "forwardButton": return controlPath;
            //gamepad bindings
            case "backButton": return controlPath;
            case "buttonSouth": return controlPath;
            case "buttonNorth": return controlPath;
            case "buttonEast": return controlPath;
            case "buttonWest": return controlPath;
            case "start": return controlPath;
            case "select": return controlPath;
            case "leftTrigger": return controlPath;
            case "rightTrigger": return controlPath;
            case "leftShoulder": return controlPath;
            case "rightShoulder": return controlPath;
            case "dpad": return controlPath;
            case "dpad/up": return controlPath;
            case "dpad/down": return controlPath;
            case "dpad/left": return controlPath;
            case "dpad/right": return controlPath;
            case "leftStick": return controlPath;
            case "rightStick": return controlPath;
            case "leftStickPress": return controlPath;
            case "rightStickPress": return controlPath;
            //if key is localization dependent (letter keys and some other specific ones) display the binding display string
            default:
                return bindingDisplayString;
        }
    }
}
