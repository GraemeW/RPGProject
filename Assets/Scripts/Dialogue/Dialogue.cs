using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Dialogue
{
    [CreateAssetMenu(fileName = "New Dialogue", menuName = "Dialogue/New Dialogue")]
    public class Dialogue : ScriptableObject
    {
        // Tunables
        [SerializeField] Vector2 newNodeOffset = new Vector2(50f, 50f);
        [SerializeField] int width = 400;
        [SerializeField] int height = 200;

        // State
        [SerializeField] List<DialogueNode> dialogueNodes = new List<DialogueNode>();
        Dictionary<string, DialogueNode> nodeLookup = new Dictionary<string, DialogueNode>();


#if UNITY_EDITOR
        private void Awake()
        {
            if (dialogueNodes.Count == 0)
            {
                DialogueNode rootNode = new DialogueNode(width, height);
                rootNode.uniqueID = System.Guid.NewGuid().ToString();
                rootNode.text = "Default Text to Overwrite";
                rootNode.isRootNode = true;
                dialogueNodes.Add(rootNode);
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
            if (parentNode == null || parentNode.children == null || parentNode.children.Count == 0) { yield break; }
            foreach (string childUniqueID in parentNode.children)
            {
                if (nodeLookup.ContainsKey(childUniqueID))
                {
                    yield return nodeLookup[childUniqueID];
                }
            }
        }

        public void CreateNode(DialogueNode parentNode)
        {
            if (parentNode == null) { return; }

            DialogueNode childNode = new DialogueNode(width, height);
            childNode.uniqueID = System.Guid.NewGuid().ToString(); ;
            childNode.text = "Default Text to Overwrite";
            childNode.rect.position += parentNode.rect.position + newNodeOffset;

            parentNode.children.Add(childNode.uniqueID);
            dialogueNodes.Add(childNode);
            OnValidate();
        }

        public void DeleteNode(DialogueNode nodeToDelete)
        {
            if (nodeToDelete == null) { return; }

            dialogueNodes.Remove(nodeToDelete);
            CleanDanglingChildren(nodeToDelete);
            OnValidate();
        }

        private void CleanDanglingChildren(DialogueNode nodeToDelete)
        {
            foreach (DialogueNode dialogueNode in dialogueNodes)
            {
                dialogueNode.children.Remove(nodeToDelete.uniqueID);
            }
        }

        public bool IsRelated(DialogueNode parentNode, DialogueNode childNode)
        {
            if (parentNode.children.Contains(childNode.uniqueID))
            {
                return true;
            }
            return false;
        }

        public void ToggleRelation(DialogueNode parentNode, DialogueNode childNode)
        {
            if (IsRelated(parentNode, childNode))
            {
                parentNode.children.Remove(childNode.uniqueID);
            }
            else
            {
                parentNode.children.Add(childNode.uniqueID);
            }
            OnValidate();
        }
    }
}
