using System;
using UnityEngine;

namespace DialogueSystem.Runtime
{
    [Serializable]
    public class DialogueNodeData
    {
        public string Guid;

        public string nameText;
        public string dialogueText;
        public Sprite illustrationSprite;
        public Vector2 position;
    }
}
