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
        static int textAreaHeight = 80;
        static float linkButtonMultiplier = 0.205f;
        static float addRemoveButtonMultiplier = 0.1f;
        static float connectionBezierOffsetMultiplier = 0.7f;
        static float connectionBezierWidth = 2f;
        const string backgroundName = "background";
        const float backgroundSize = 50f;

        // State
        [NonSerialized] GUIStyle nodeStyle = null;
        [NonSerialized]DialogueNode draggingNode = null;
        [NonSerialized] Vector2 draggingOffset = new Vector2();
        [NonSerialized] DialogueNode creatingNode = null;
        [NonSerialized] DialogueNode deletingNode = null;
        [NonSerialized] DialogueNode linkingParentNode = null;
        [NonSerialized] Vector2 scrollPosition = new Vector2();
        [NonSerialized] float scrollMaxX = 1;
        [NonSerialized] float scrollMaxY = 1;

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

                scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
                DrawBackground();
                foreach (DialogueNode dialogueNode in selectedDialogue.GetAllNodes())
                {
                    DrawConnections(dialogueNode);
                }
                foreach (DialogueNode dialogueNode in selectedDialogue.GetAllNodes())
                {
                    DrawNode(dialogueNode);
                }
                EditorGUILayout.EndScrollView();

                if (deletingNode != null)
                {
                    Undo.RecordObject(selectedDialogue, "Delete Dialogue Node");
                    selectedDialogue.DeleteNode(deletingNode);
                    deletingNode = null;
                }
                if (creatingNode != null)
                {
                    Undo.RecordObject(selectedDialogue, "Add Dialogue Node");
                    DialogueNode newChildNode = selectedDialogue.CreateChildNode(creatingNode);
                    creatingNode = null;
                }
            }
        }

        private void DrawBackground()
        {
            Rect canvas = GUILayoutUtility.GetRect(scrollMaxX, scrollMaxY);
            Texture2D backgroundTexture = Resources.Load(backgroundName) as Texture2D;
            GUI.DrawTextureWithTexCoords(canvas, backgroundTexture, new Rect(0, 0, canvas.width / backgroundSize, canvas.height / backgroundSize));

            // Reset scrolling limits, to be updated after draw nodes
            scrollMaxX = 1f;
            scrollMaxX = 1f;
        }

        private void ProcessEvents()
        {
            if (Event.current.type == EventType.MouseDown)
            {
                draggingNode = null;
                draggingOffset = new Vector2();

                Vector2 mousePosition = Event.current.mousePosition;
                draggingNode = GetNodeAtPoint(mousePosition + scrollPosition);
                if (draggingNode != null)
                {
                    draggingOffset = draggingNode.rect.position - mousePosition;
                    Selection.activeObject = draggingNode;
                }
                else
                {
                    draggingOffset = mousePosition + scrollPosition;
                    Selection.activeObject = selectedDialogue;
                }
            }
            else if (Event.current.type == EventType.MouseDrag && draggingNode != null)
            {
                Vector2 currentMousePosition = Event.current.mousePosition;
                Undo.RecordObject(selectedDialogue, "Move Dialogue Node");
                draggingNode.rect.position = currentMousePosition + draggingOffset;
                GUI.changed = true;
            }
            else if (Event.current.type == EventType.MouseDrag && draggingNode == null)
            {
                scrollPosition = draggingOffset - Event.current.mousePosition;
                GUI.changed = true;
            }
            else if (Event.current.type == EventType.MouseUp)
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
            EditorGUILayout.LabelField("Unique ID:", dialogueNode.name);

            // Text Input
            EditorGUILayout.BeginScrollView(Vector2.zero);
            EditorGUI.BeginChangeCheck();
            string newText = EditorGUILayout.TextArea(dialogueNode.text, 
                GUILayout.Width(dialogueNode.rect.width - nodePadding*2 - nodeBorder*2), 
                GUILayout.Height(textAreaHeight));
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(selectedDialogue, "Update Dialogue");
                dialogueNode.text = newText;
            }
            EditorGUILayout.EndScrollView();

            // Additional Functionality
            GUILayout.FlexibleSpace();
            DrawLinkButtons(dialogueNode);
            DrawAddRemoveButtons(dialogueNode);

            GUILayout.EndArea();

            UpdateMaxScrollViewDimensions(dialogueNode);
        }

        private void UpdateMaxScrollViewDimensions(DialogueNode dialogueNode)
        {
            scrollMaxX = Mathf.Max(scrollMaxX, dialogueNode.rect.xMax);
            scrollMaxY = Mathf.Max(scrollMaxY, dialogueNode.rect.yMax);
        }

        private void DrawAddRemoveButtons(DialogueNode dialogueNode)
        {
            // Set tags to create/delete at end of OnGUI to avoid operating on list while iterating over it
            GUILayout.BeginHorizontal();
            if (!dialogueNode.GetRootNode())
            {
                if (GUILayout.Button("-", GUILayout.Width(dialogueNode.rect.width * addRemoveButtonMultiplier)))
                {
                    deletingNode = dialogueNode;
                }
            }

            if (GUILayout.Button("+", GUILayout.Width(dialogueNode.rect.width * addRemoveButtonMultiplier)))
            {
                creatingNode = dialogueNode;
            }
            GUILayout.EndHorizontal();
        }

        private void DrawLinkButtons(DialogueNode dialogueNode)
        {
            if (linkingParentNode == null)
            {
                if (GUILayout.Button("link", GUILayout.Width(dialogueNode.rect.width * linkButtonMultiplier)))
                {
                    linkingParentNode = dialogueNode;
                }
            }
            else
            {
                if (dialogueNode != linkingParentNode)
                {
                    string buttonText = "child";
                    if (selectedDialogue.IsRelated(linkingParentNode, dialogueNode)) { buttonText = "unlink"; }

                    if (GUILayout.Button(buttonText, GUILayout.Width(dialogueNode.rect.width * linkButtonMultiplier)))
                    {
                        Undo.RecordObject(selectedDialogue, "Add Dialogue Relation");
                        selectedDialogue.ToggleRelation(linkingParentNode, dialogueNode);
                        linkingParentNode = null;
                    }
                }
                else
                {
                    if (GUILayout.Button("---", GUILayout.Width(dialogueNode.rect.width * linkButtonMultiplier)))
                    {
                        linkingParentNode = null;
                    }
                }
            }
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