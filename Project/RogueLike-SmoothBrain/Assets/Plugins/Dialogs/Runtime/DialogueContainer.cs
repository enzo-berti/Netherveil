using System.Collections.Generic;
using UnityEngine;

namespace DialogueSystem.Runtime
{
    public class DialogueContainer : ScriptableObject
    {
        public List<NodeLinkData> nodeLinks = new List<NodeLinkData>();
        public List<DialogueNodeData> dialogueNodeData = new List<DialogueNodeData>();
        public List<ExposedProperty> exposedProperties = new List<ExposedProperty>();
    }
}
