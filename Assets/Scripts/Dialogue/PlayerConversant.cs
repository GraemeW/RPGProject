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

        private void Start() // HACK, TODO:  Remove, implement intiiation through player events
        {
            InitiateConversation();
        }

        private void InitiateConversation()
        {
            currentNode = currentDialogue.GetRootNode();
            if (dialogueInitiated != null)
            {
                dialogueInitiated();
            }
        }

        public SpeakerType GetCurrentSpeaker()
        {
            return currentNode.GetSpeaker();
        }

        public SpeakerType GetNextSpeaker()
        {
            return currentDialogue.GetNodeFromID(currentNode.GetChildren()[0]).GetSpeaker();
        }

        public int GetChoiceCount()
        {
            return currentNode.GetChildren().Count;
        }

        public string GetCurrentSpeakerName()
        {
            return currentNode.GetSpeakerName();
        }

        public string GetText()
        {
            return currentNode.GetText();
        }

        public IEnumerable<DialogueNode> GetChoices()
        {
            foreach (string childID in currentNode.GetChildren())
            {
                yield return currentDialogue.GetNodeFromID(childID);
            }
        }

        public bool HasNext()
        {
            return currentNode.GetChildren().Count > 0;
        }

        public void NextWithID(string nodeID)
        {
            if (HasNext())
            {
                currentNode = currentDialogue.GetNodeFromID(nodeID);
                if (dialogueUpdated != null)
                {
                    dialogueUpdated();
                }
            }
        }

        public void NextRandom()
        {
            if (HasNext())
            {
                int nodeIndex = UnityEngine.Random.Range(0, currentNode.GetChildren().Count);
                currentNode = currentDialogue.GetNodeFromID(currentNode.GetChildren()[nodeIndex]);
                if (dialogueUpdated != null)
                {
                    dialogueUpdated();
                }
            }
        }

        public void EndConversation()
        {
            currentDialogue = null;
            currentNode = null;
            if (dialogueEnded != null)
            {
                dialogueEnded();
            }
        }
    }
}