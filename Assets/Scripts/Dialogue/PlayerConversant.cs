using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Dialogue
{
    public class PlayerConversant : MonoBehaviour
    {
        // Tunables
        [SerializeField] Dialogue testDialogue = null;

        // State
        Dialogue currentDialogue = null;
        DialogueNode currentNode = null;

        // Events
        public event Action dialogueUpdated;

        IEnumerator Start() // HACK, TODO:  Remove, implement intiiation through player events
        {
            yield return new WaitForSeconds(2);
            InitiateConversation(testDialogue);
        }

        private void InitiateConversation(Dialogue newDialogue)
        {
            currentDialogue = newDialogue;
            currentNode = currentDialogue.GetRootNode();
            if (dialogueUpdated != null)
            {
                dialogueUpdated();
            }
        }

        public bool IsActive()
        {
            return (currentDialogue != null && currentNode != null);
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

        public bool IsChoosing()
        {
            return (GetChoiceCount() > 1 && GetNextSpeaker() == SpeakerType.player);
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
            if (dialogueUpdated != null)
            {
                dialogueUpdated();
            }
        }
    }
}