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
        [SerializeField] Vector2 newNodeOffset = new Vector2(50f, 50f);
        [SerializeField] int nodeWidth = 400;
        [SerializeField] int nodeHeight = 200;

        // State
        List<DialogueNode> dialogueNodes = new List<DialogueNode>();
        Dictionary<string, DialogueNode> nodeLookup = new Dictionary<string, DialogueNode>();


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
            if (parentNode == null || parentNode.children == null || parentNode.children.Count == 0) { yield break; }
            foreach (string childUniqueID in parentNode.children)
            {
                if (nodeLookup.ContainsKey(childUniqueID))
                {
                    yield return nodeLookup[childUniqueID];
                }
            }
        }

        private DialogueNode CreateNode()
        {
            DialogueNode dialogueNode = CreateInstance<DialogueNode>();
            dialogueNode.Initialize(nodeWidth, nodeHeight);
            dialogueNode.name = System.Guid.NewGuid().ToString(); ;
            dialogueNode.text = "Default Text to Overwrite";

#if UNITY_EDITOR
            Undo.RegisterCreatedObjectUndo(dialogueNode, "Created Dialogue Node Object");
#endif

            dialogueNodes.Add(dialogueNode);
            return dialogueNode;
        }

        public DialogueNode CreateChildNode(DialogueNode parentNode)
        {
            if (parentNode == null) { return null; }

            DialogueNode childNode = CreateNode();
            childNode.rect.position += parentNode.rect.position + newNodeOffset;
            parentNode.children.Add(childNode.name);
            OnValidate();

            return childNode;
        }

        private DialogueNode CreateRootNodeIfMissing()
        {
            if (dialogueNodes.Count == 0)
            {
                DialogueNode rootNode = CreateNode();
                rootNode.SetRootNode(true);

                OnValidate();
                return rootNode;
            }

            return null;
        }

        public void DeleteNode(DialogueNode nodeToDelete)
        {
            if (nodeToDelete == null) { return; }

            dialogueNodes.Remove(nodeToDelete);
            CleanDanglingChildren(nodeToDelete);
            OnValidate();

#if UNITY_EDITOR
            Undo.DestroyObjectImmediate(nodeToDelete);
#endif
        }

        private void CleanDanglingChildren(DialogueNode nodeToDelete)
        {
            foreach (DialogueNode dialogueNode in dialogueNodes)
            {
                dialogueNode.children.Remove(nodeToDelete.name);
            }
        }

        public bool IsRelated(DialogueNode parentNode, DialogueNode childNode)
        {
            if (parentNode.children.Contains(childNode.name))
            {
                return true;
            }
            return false;
        }

        public void ToggleRelation(DialogueNode parentNode, DialogueNode childNode)
        {
            if (IsRelated(parentNode, childNode))
            {
                parentNode.children.Remove(childNode.name);
            }
            else
            {
                parentNode.children.Add(childNode.name);
            }
            OnValidate();
        }

#if UNITY_EDITOR
        public void OnBeforeSerialize()
        {
            CreateRootNodeIfMissing();

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
        }

        public void OnAfterDeserialize() // Unused, required for interface
        {
        }
#endif
    }
}
