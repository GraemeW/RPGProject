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
                DialogueNode rootNode = new DialogueNode();
                rootNode.uniqueID = System.Guid.NewGuid().ToString();
                rootNode.text = "Default Text to Overwrite";
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

            DialogueNode childNode = new DialogueNode();
            childNode.uniqueID = System.Guid.NewGuid().ToString(); ;
            childNode.text = "Default Text to Overwrite";

            parentNode.children.Add(childNode.uniqueID);
            dialogueNodes.Add(childNode);
            OnValidate();
        }
    }
}
