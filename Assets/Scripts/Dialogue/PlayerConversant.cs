using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Dialogue
{
    public class PlayerConversant : MonoBehaviour
    {
        // Tunable
        [SerializeField] string playerName = "";

        // State
        Dialogue currentDialogue = null;
        DialogueNode currentNode = null;
        AIConversant currentConversant = null;

        // Events
        public event Action dialogueUpdated;

        // Methods
        public string GetPlayerName()
        {
            return playerName;
        }

        public void InitiateConversation(AIConversant newConversant, Dialogue newDialogue)
        {
            currentConversant = newConversant;
            currentDialogue = newDialogue;
            currentDialogue.SetPlayer(this);
            currentDialogue.OverrideSpeakers();

            currentNode = currentDialogue.GetRootNode();
            TriggerEnterAction();
            if (dialogueUpdated != null)
            {
                dialogueUpdated();
            }
        }

        public void EndConversation()
        {
            TriggerExitAction();
            currentConversant = null;
            currentDialogue = null;
            currentNode = null;
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
                TriggerExitAction();
                currentNode = currentDialogue.GetNodeFromID(nodeID);
                TriggerEnterAction();
                if (HasNext()) // Skip re-showing the player choice
                {
                    Next(); 
                }
                else // Unless it's the last stem in the dialogue tree
                {
                    if (dialogueUpdated != null)
                    {
                        dialogueUpdated();
                        TriggerEnterAction();
                    }
                }
            }
        }

        public void Next()
        {
            if (HasNext())
            {
                int nodeIndex = UnityEngine.Random.Range(0, currentNode.GetChildren().Count);
                TriggerExitAction();
                currentNode = currentDialogue.GetNodeFromID(currentNode.GetChildren()[nodeIndex]);
                TriggerEnterAction();
                if (dialogueUpdated != null)
                {
                    dialogueUpdated();
                }
            }
        }

        private void TriggerEnterAction()
        {
            TriggerAction(currentNode.GetOnEnterAction());
        }

        private void TriggerExitAction()
        {
            TriggerAction(currentNode.GetOnExitAction());
        }

        private void TriggerAction(string action)
        {
            if (currentNode != null && !string.IsNullOrWhiteSpace(action))
            {
                DialogueTrigger[] dialogueTriggers = currentConversant.GetComponents<DialogueTrigger>();
                foreach (DialogueTrigger dialogueTrigger in dialogueTriggers)
                {
                    dialogueTrigger.Trigger(action);
                }
            }
        }
    }
}