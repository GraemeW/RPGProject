using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Dialogue
{
    [CreateAssetMenu(fileName = "New Dialogue", menuName = "Dialogue/New Dialogue")]
    public class Dialogue : ScriptableObject
    {
        [SerializeField] List<DialogueNode> dialogueNodes = new List<DialogueNode>();
        [SerializeField] Dictionary<string, DialogueNode> nodeLookup = new Dictionary<string, DialogueNode>();


#if UNITY_EDITOR
        private void Awake()
        {
            if (dialogueNodes.Count == 0)
            {
                DialogueNode blankDialogueNode = new DialogueNode();
                dialogueNodes.Add(blankDialogueNode);
            }
            OnValidate();
        }
#endif

        private void OnValidate()
        {
            nodeLookup = new Dictionary<string, DialogueNode>();
            foreach (DialogueNode dialogueNode in dialogueNodes)
            {
                nodeLookup.Add(dialogueNode.uniqueID, dialogueNode);
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

        public DialogueNode GetNodeFromID(string uniqueID)
        {
            foreach (DialogueNode dialogueNode in dialogueNodes)
            {
                if (dialogueNode.uniqueID == uniqueID)
                {
                    return dialogueNode;
                }
            }
            return null;
        }

        public IEnumerable<DialogueNode> GetAllChildren(DialogueNode parentNode)
        {
            if (parentNode == null || parentNode.children == null || parentNode.children.Length == 0) { yield break; }
            foreach (string childUniqueID in parentNode.children)
            {
                if (nodeLookup.ContainsKey(childUniqueID))
                {
                    yield return nodeLookup[childUniqueID];
                }
            }
        }
    }
}
