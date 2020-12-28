using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Dialogue
{
    [System.Serializable]
    public class DialogueNode : ScriptableObject
    {
        public string text = "";
        public List<string> children = new List<string>();
        public Rect rect = new Rect(30, 30, 400, 200);
        bool isRootNode = false;

        public void Initialize(int width, int height)
        {
            rect.width = width;
            rect.height = height;
        }

        public void SetRootNode(bool isRoot)
        {
            isRootNode = isRoot;
        }

        public bool GetRootNode()
        {
            return isRootNode;
        }
    }
}
