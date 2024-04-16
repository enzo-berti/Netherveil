using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class DialogueTreeEventManager : MonoBehaviour
{
    [System.Serializable]
    public class DialogueEvent
    {
        public DialogueEvent(string tag, UnityAction action)
        {
            this.tag = tag;
            onCall.AddListener(action);
        }

        public string tag;
        public UnityEvent onCall = new();
    }

    [SerializeField] private List<DialogueEvent> dialogueEvent = new List<DialogueEvent>();

    public void Invoke(string tag)
    {
        dialogueEvent.First(x => x.tag == tag).onCall.Invoke();
    }

    public void AddListener(string tag, UnityAction action)
    {
        if (dialogueEvent.FirstOrDefault(x => x.tag == tag) != null)
        {
            Debug.LogWarning($"The tag {tag} is already use for an event.");
            return;
        }

        dialogueEvent.Add(new DialogueEvent(tag, action));
    }

    public void RemoveListener(string tag)
    {
        dialogueEvent.Remove(dialogueEvent.First(x => x.tag == tag));
    }
}
