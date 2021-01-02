using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using RPG.Core;

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
            if (currentDialogue.skipRootNode) { Next(false); }
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
            return FilterOnCondition(currentNode.GetChildren()).Count();
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
            foreach (string childID in FilterOnCondition(currentNode.GetChildren()))
            {
                yield return currentDialogue.GetNodeFromID(childID);
            }
        }

        public bool HasNext()
        {
            return FilterOnCondition(currentNode.GetChildren()).Count() > 0;
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

        public void Next(bool withTriggers = true)
        {
            if (HasNext())
            {
                List<string> filteredDialogueOptions = FilterOnCondition(currentNode.GetChildren()).ToList();
                int nodeIndex = UnityEngine.Random.Range(0, filteredDialogueOptions.Count);
                if (withTriggers) { TriggerExitAction(); }
                currentNode = currentDialogue.GetNodeFromID(filteredDialogueOptions[nodeIndex]);
                if (withTriggers) { TriggerEnterAction(); }
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

        private IEnumerable<string> FilterOnCondition(List<string> dialogueNodeIDs)
        {
            foreach (string dialogueNodeID in dialogueNodeIDs)
            {
                if (currentDialogue.GetNodeFromID(dialogueNodeID).CheckCondition(GetEvaluators()))
                {
                    yield return dialogueNodeID;
                }
            }
        }

        private IEnumerable<IPredicateEvaluator> GetEvaluators()
        {
            return GetComponents<IPredicateEvaluator>().Concat(
                currentConversant.GetComponents<IPredicateEvaluator>());
        }
    }
}