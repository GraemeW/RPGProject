using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace RPG.Dialogue.Editor
{
    public class DialogueEditor : EditorWindow
    {
        // Tunables
        Dialogue selectedDialogue = null;
        GUIStyle nodeStyle = null;
        int nodePadding = 20;
        int nodeBorder = 12;

        // State
        DialogueNode draggingNode = null;
        Vector2 draggingOffset = new Vector2();

        [MenuItem("Window/Dialogue Editor")]
        public static void ShowEditorWindow()
        {
            GetWindow(typeof(DialogueEditor), false, "Dialogue Editor");
        }

        [OnOpenAsset(1)]
        public static bool OnOpenAsset(int instanceID, int line)
        {
            Dialogue dialogue = EditorUtility.InstanceIDToObject(instanceID) as Dialogue;
            if (dialogue != null)
            {
                ShowEditorWindow();
                return true;
            }
            return false;
        }

        private void OnEnable()
        {
            Selection.selectionChanged += OnSelectionChanged;
            SetupNodeStyle();
        }

        private void SetupNodeStyle()
        {
            nodeStyle = new GUIStyle();
            nodeStyle.normal.background = EditorGUIUtility.Load("node0") as Texture2D;
            nodeStyle.padding = new RectOffset(nodePadding, nodePadding, nodePadding, nodePadding);
            nodeStyle.border = new RectOffset(nodeBorder, nodeBorder, nodeBorder, nodeBorder);
        }

        private void OnSelectionChanged()
        {
            Dialogue newDialogue =  Selection.activeObject as Dialogue;
            if (newDialogue != null)
            {
                selectedDialogue = newDialogue;
                Repaint();
            }
        }

        private void OnGUI()
        {
            if (selectedDialogue == null)
            {
                EditorGUILayout.LabelField("No dialogue selected.");
            }
            else
            {
                ProcessEvents();

                EditorGUILayout.LabelField(selectedDialogue.name);
                foreach (DialogueNode dialogueNode in selectedDialogue.GetAllNodes())
                {
                    OnGUINode(dialogueNode);
                }
            }
        }

        private void ProcessEvents()
        {
            if (Event.current.type == EventType.MouseDown && draggingNode == null)
            {
                draggingNode = GetNodeAtPoint(Event.current.mousePosition);
                if (draggingNode != null)
                {
                    draggingOffset = draggingNode.rect.position - Event.current.mousePosition;
                }
            }
            else if (Event.current.type == EventType.MouseDrag && draggingNode != null)
            {
                Vector2 currentMousePosition = Event.current.mousePosition;
                Undo.RecordObject(selectedDialogue, "Move Dialogue Node");
                draggingNode.rect.position = currentMousePosition + draggingOffset;
                GUI.changed = true;
            }
            else if (Event.current.type == EventType.MouseUp && draggingNode != null)
            {
                draggingNode = null;
                draggingOffset = new Vector2();
            }
        }

        private void OnGUINode(DialogueNode dialogueNode)
        {
            GUILayout.BeginArea(dialogueNode.rect, nodeStyle);

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.LabelField("Node: ");
            string newUniqueID = EditorGUILayout.TextField("Unique ID: ", dialogueNode.uniqueID);
            string newText = EditorGUILayout.TextField("Text: ", dialogueNode.text);

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(selectedDialogue, "Update Dialogue");
                dialogueNode.uniqueID = newUniqueID;
                dialogueNode.text = newText;
            }

            GUILayout.EndArea();
        }

        private DialogueNode GetNodeAtPoint(Vector2 point)
        {
            DialogueNode foundNode = null;
            foreach (DialogueNode dialogueNode in selectedDialogue.GetAllNodes())
            {
                if (dialogueNode.rect.Contains(point))
                {
                    foundNode = dialogueNode;
                }
            }
            return foundNode;
        }
    }
}