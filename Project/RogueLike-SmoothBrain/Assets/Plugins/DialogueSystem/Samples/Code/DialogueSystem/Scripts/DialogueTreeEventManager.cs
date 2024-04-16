using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class DialogueTreeEventManager : MonoBehaviour
{
    [System.Serializable]
    public class DialogueEvent
    {
        public string tag;
        public UnityEvent onCall;
    }

    [SerializeField] private List<DialogueEvent> dialogueEvent = new List<DialogueEvent>();

    public void Invoke(string tag)
    {
        dialogueEvent.First(x => x.tag == tag).onCall.Invoke();
    }
}
