using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace RPG.Dialogue
{
    [System.Serializable]
    public class DialogueNode : ScriptableObject
    {
        [SerializeField] SpeakerType speaker = SpeakerType.speakerOne;
        [SerializeField] string text = "";
        [SerializeField] List<string> children = new List<string>();
        [SerializeField] Rect rect = new Rect(30, 30, 400, 200);
        [HideInInspector][SerializeField] Rect draggingRect = new Rect(0, 0, 400, 45);
        [HideInInspector][SerializeField] bool isRootNode = false;

        public SpeakerType GetSpeaker()
        {
            return speaker;
        }
        public string GetText()
        {
            return text;
        }

        public List<string> GetChildren()
        {
            return children;
        }

        public Vector2 GetPosition()
        {
            return rect.position;
        }

        public Rect GetRect()
        {
            return rect;
        }

        public Rect GetDraggingRect()
        {
            return draggingRect;
        }

        public bool IsRootNode()
        {
            return isRootNode;
        }


#if UNITY_EDITOR
        public void Initialize(int width, int height)
        {
            rect.width = width;
            rect.height = height;
            EditorUtility.SetDirty(this);
        }

        public void SetSpeaker(SpeakerType speaker)
        {
            if (speaker != this.speaker)
            {
                Undo.RecordObject(this, "Update Dialogue Speaker");
                this.speaker = speaker;
                EditorUtility.SetDirty(this);
            }
        }

        public void SetText(string text)
        {
            if (text != this.text)
            {
                Undo.RecordObject(this, "Update Dialogue");
                this.text = text;
                EditorUtility.SetDirty(this);
            }
        }

        public void AddChild(string childID)
        {
            Undo.RecordObject(this, "Add Node Relation");
            children.Add(childID);
            EditorUtility.SetDirty(this);
        }

        public void RemoveChild(string childID)
        {
            Undo.RecordObject(this, "Remove Node Relation");
            children.Remove(childID);
            EditorUtility.SetDirty(this);
        }

        public void SetPosition(Vector2 position)
        {
            Undo.RecordObject(this, "Move Dialogue Node");
            rect.position = position;
            EditorUtility.SetDirty(this);
        }

        public void SetDraggingRect(Rect draggingRect)
        {
            if (draggingRect != this.draggingRect)
            {
                this.draggingRect = draggingRect;
                EditorUtility.SetDirty(this);
            }
        }

        public void SetRootNode(bool isRoot)
        {
            isRootNode = isRoot;
            EditorUtility.SetDirty(this);
        }
#endif
    }
}
