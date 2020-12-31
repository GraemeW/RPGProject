using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace RPG.Dialogue
{
    [CreateAssetMenu(fileName = "New Dialogue", menuName = "Dialogue/New Dialogue")]
    public class Dialogue : ScriptableObject, ISerializationCallbackReceiver
    {
        // Tunables
        [Header("Speaker Overrides")]
        PlayerConversant speakerPlayer = null; // Set via script
        [SerializeField] AIConversant speakerOne = null;
        [SerializeField] AIConversant speakerTwo = null;
        [SerializeField] AIConversant speakerThree = null;
        [SerializeField] AIConversant speakerFour = null;

        [Header("Editor Settings")]
        [SerializeField] Vector2 newNodeOffset = new Vector2(100f, 25f);
        [SerializeField] int nodeWidth = 400;
        [SerializeField] int nodeHeight = 225;

        // State
        [HideInInspector][SerializeField] List<DialogueNode> dialogueNodes = new List<DialogueNode>();
        [HideInInspector][SerializeField] Dictionary<string, DialogueNode> nodeLookup = new Dictionary<string, DialogueNode>();

#if UNITY_EDITOR
        private void Awake()
        {
            CreateRootNodeIfMissing();
        }
#endif

        private void OnValidate()
        {
            nodeLookup = new Dictionary<string, DialogueNode>();
            foreach (DialogueNode dialogueNode in dialogueNodes)
            {
                nodeLookup.Add(dialogueNode.name, dialogueNode);
            }
        }

        public IEnumerable<DialogueNode> GetAllNodes()
        {
            return dialogueNodes;
        }

        public DialogueNode GetRootNode()
        {
            return dialogueNodes[0];
        }

        public DialogueNode GetNodeFromID(string name)
        {
            foreach (DialogueNode dialogueNode in dialogueNodes)
            {
                if (dialogueNode.name == name)
                {
                    return dialogueNode;
                }
            }
            return null;
        }

        public IEnumerable<DialogueNode> GetAllChildren(DialogueNode parentNode)
        {
            if (parentNode == null || parentNode.GetChildren() == null || parentNode.GetChildren().Count == 0) { yield break; }
            foreach (string childUniqueID in parentNode.GetChildren())
            {
                if (nodeLookup.ContainsKey(childUniqueID))
                {
                    yield return nodeLookup[childUniqueID];
                }
            }
        }

        public bool IsRelated(DialogueNode parentNode, DialogueNode childNode)
        {
            if (parentNode.GetChildren().Contains(childNode.name))
            {
                return true;
            }
            return false;
        }

        public void SetPlayer(PlayerConversant playerConversant)
        {
            speakerPlayer = playerConversant;
        }

        public void SetSpeaker(SpeakerType speakerType, AIConversant aiConversant)
        {
            if (speakerType == SpeakerType.speakerOne) { speakerOne = aiConversant; }
            else if (speakerType == SpeakerType.speakerTwo) { speakerTwo = aiConversant; }
            else if (speakerType == SpeakerType.speakerThree) { speakerThree = aiConversant; }
            else if (speakerType == SpeakerType.speakerFour) { speakerFour = aiConversant; }
        }

        private bool SpeakerNameOverride(SpeakerType speakerType)
        {
            if (speakerType == SpeakerType.player) { return (speakerPlayer != null && !string.IsNullOrWhiteSpace(speakerPlayer.GetPlayerName())); }
            if (speakerType == SpeakerType.speakerOne) { return (speakerOne != null && !string.IsNullOrWhiteSpace(speakerOne.GetConversantName())); }
            if (speakerType == SpeakerType.speakerTwo) { return (speakerTwo != null && !string.IsNullOrWhiteSpace(speakerTwo.GetConversantName())); }
            if (speakerType == SpeakerType.speakerThree) { return (speakerThree != null && !string.IsNullOrWhiteSpace(speakerThree.GetConversantName())); }
            if (speakerType == SpeakerType.speakerFour) { return (speakerFour != null && !string.IsNullOrWhiteSpace(speakerFour.GetConversantName())); }
            return false;
        }

        public void OverrideSpeakers()
        {
            foreach (DialogueNode dialogueNode in dialogueNodes)
            {
                SpeakerType speakerType = dialogueNode.GetSpeaker();
                string nameOverride = "";
                if (speakerType == SpeakerType.player && SpeakerNameOverride(speakerType)) { nameOverride = speakerPlayer.GetPlayerName(); }
                else if (speakerType == SpeakerType.speakerOne && SpeakerNameOverride(speakerType)) { nameOverride = speakerOne.GetConversantName(); }
                else if (speakerType == SpeakerType.speakerTwo && SpeakerNameOverride(speakerType)) { nameOverride = speakerTwo.GetConversantName(); }
                else if (speakerType == SpeakerType.speakerThree && SpeakerNameOverride(speakerType)) { nameOverride = speakerThree.GetConversantName(); }
                else if (speakerType == SpeakerType.speakerFour && SpeakerNameOverride(speakerType)) { nameOverride = speakerFour.GetConversantName(); }

                dialogueNode.SetSpeakerName(nameOverride);
            }
        }

        // Dialogue editing functionality
#if UNITY_EDITOR
        private DialogueNode CreateNode()
        {
            DialogueNode dialogueNode = CreateInstance<DialogueNode>();
            Undo.RegisterCreatedObjectUndo(dialogueNode, "Created Dialogue Node Object");
            dialogueNode.Initialize(nodeWidth, nodeHeight);
            dialogueNode.name = System.Guid.NewGuid().ToString();
            dialogueNode.SetText("Default Text to Overwrite");

            Undo.RecordObject(this, "Add Dialogue Node");
            dialogueNodes.Add(dialogueNode);

            return dialogueNode;
        }

        public DialogueNode CreateChildNode(DialogueNode parentNode)
        {
            if (parentNode == null) { return null; }

            DialogueNode childNode = CreateNode();
            Vector2 offsetPosition = new Vector2(parentNode.GetRect().xMax + newNodeOffset.x, 
                parentNode.GetRect().yMin + (parentNode.GetRect().height + newNodeOffset.y)* parentNode.GetChildren().Count);
            childNode.SetPosition(offsetPosition);
            if (parentNode.GetSpeaker() == SpeakerType.player)
            {
                childNode.SetSpeaker(SpeakerType.speakerOne);
            }
            else
            {
                childNode.SetSpeaker(SpeakerType.player);
            }

            parentNode.AddChild(childNode.name);
            OnValidate();

            return childNode;
        }

        private DialogueNode CreateRootNodeIfMissing()
        {
            if (dialogueNodes.Count == 0)
            {
                DialogueNode rootNode = CreateNode();

                OnValidate();
                return rootNode;
            }

            return null;
        }

        public void DeleteNode(DialogueNode nodeToDelete)
        {
            if (nodeToDelete == null) { return; }

            Undo.RecordObject(this, "Delete Dialogue Node");
            dialogueNodes.Remove(nodeToDelete);
            CleanDanglingChildren(nodeToDelete);
            OnValidate();

            Undo.DestroyObjectImmediate(nodeToDelete);
        }

        private void CleanDanglingChildren(DialogueNode nodeToDelete)
        {
            foreach (DialogueNode dialogueNode in dialogueNodes)
            {
                dialogueNode.RemoveChild(nodeToDelete.name);
            }
        }

        public void ToggleRelation(DialogueNode parentNode, DialogueNode childNode)
        {
            if (IsRelated(parentNode, childNode))
            {
                parentNode.RemoveChild(childNode.name);
            }
            else
            {
                parentNode.AddChild(childNode.name);
            }
            OnValidate();
        }

        public void UpdateSpeakerName(SpeakerType newSpeakerType, string newSpeakerName)
        {
            foreach (DialogueNode dialogueNode in dialogueNodes)
            {
                if (dialogueNode.GetSpeaker() == newSpeakerType)
                {
                    dialogueNode.SetSpeakerName(newSpeakerName);
                }
            }
        }
#endif

        public void OnBeforeSerialize()
        {
#if UNITY_EDITOR
            if (AssetDatabase.GetAssetPath(this) != "")
            {
                foreach (DialogueNode dialogueNode in GetAllNodes())
                {
                    if (AssetDatabase.GetAssetPath(dialogueNode) == "")
                    {
                        AssetDatabase.AddObjectToAsset(dialogueNode, this);
                    }
                }
            }
#endif
        }

        public void OnAfterDeserialize() // Unused, required for interface
        {
        }
    }
}
