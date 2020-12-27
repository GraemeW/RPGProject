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
        static int labelOffset = 80;
        static int nodePadding = 20;
        static int nodeBorder = 12;
        static float connectionBezierOffsetMultiplier = 0.7f;
        static float connectionBezierWidth = 2f;

        // State
        [NonSerialized] GUIStyle nodeStyle = null;
        [NonSerialized]DialogueNode draggingNode = null;
        [NonSerialized] Vector2 draggingOffset = new Vector2();
        [NonSerialized] DialogueNode creatingNode = null;
        
        public object DrawBezier { get; private set; }

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
                    DrawConnections(dialogueNode);
                }
                foreach (DialogueNode dialogueNode in selectedDialogue.GetAllNodes())
                {
                    DrawNode(dialogueNode);
                }

                if (creatingNode != null)
                {
                    Undo.RecordObject(selectedDialogue, "Add Dialogue Node");
                    selectedDialogue.CreateNode(creatingNode);
                    creatingNode = null;
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

        private void DrawNode(DialogueNode dialogueNode)
        {
            GUILayout.BeginArea(dialogueNode.rect, nodeStyle);

            // Node Properties
            EditorGUIUtility.labelWidth = labelOffset;
            EditorGUILayout.LabelField("Unique ID:", dialogueNode.uniqueID);
            EditorGUI.BeginChangeCheck();
            string newText = EditorGUILayout.TextField("Text: ", dialogueNode.text);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(selectedDialogue, "Update Dialogue");
                dialogueNode.text = newText;
            }

            // Additional Functionality
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("+", GUILayout.Width(dialogueNode.rect.width * 0.25f)))
            {
                creatingNode = dialogueNode; // Call node creation after loop finished in OnGUI()
            }
            
            GUILayout.EndArea();
        }

        private void DrawConnections(DialogueNode dialogueNode)
        {
            Vector2 startPoint = new Vector2(dialogueNode.rect.xMax, dialogueNode.rect.center.y);
            foreach (DialogueNode childNode in selectedDialogue.GetAllChildren(dialogueNode))
            {
                Vector2 endPoint = new Vector2(childNode.rect.xMin, childNode.rect.center.y);
                float connectionBezierOffset = (endPoint.x - startPoint.x) * connectionBezierOffsetMultiplier;
                Handles.DrawBezier(startPoint, endPoint, 
                    startPoint + Vector2.right * connectionBezierOffset, endPoint + Vector2.left * connectionBezierOffset, 
                    Color.white, null, connectionBezierWidth);
            }
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