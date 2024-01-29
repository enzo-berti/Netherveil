using DialogueSystem.Runtime;
using UnityEngine;

public class Merchant : Npc, IInterractable
{
    [Header("Merchant parameters")]
    [SerializeField] private DialogueContainer dialogue;

    public void Interract()
    {
        TriggerDialogue();
    }

    private void TriggerDialogue()
    {
        FindObjectOfType<DialogueManager>().StartDialogue(dialogue);
    }
}
