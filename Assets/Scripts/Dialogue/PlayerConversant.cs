using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Dialogue
{
    public class PlayerConversant : MonoBehaviour
    {
        // Tunables
        [SerializeField] Dialogue currentDialogue = null;

        // State
        DialogueNode currentNode = null;

        // Events
        public event Action dialogueInitiated;
        public event Action dialogueUpdated;
        public event Action dialogueEnded;

        private void Start()
        {
            InitiateConversation();
        }

        private void InitiateConversation()
        {
            currentNode = currentDialogue.GetRootNode();
            dialogueInitiated();
        }

        public string GetCurrentSpeakerName()
        {
            return currentNode.GetSpeakerName();
        }

        public string GetText()
        {
            return currentNode.GetText();
        }
    }
}