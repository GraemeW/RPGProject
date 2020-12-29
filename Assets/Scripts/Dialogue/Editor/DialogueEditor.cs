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
        [NonSerialized] DialogueNode selectedNode = null;
        [NonSerialized] bool draggable = false;
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
            nodeStyle.padding = new RectOffset(nodePadding, nodePadding, nodePadding / 2, nodePadding);
            nodeStyle.border = new RectOffset(nodeBorder, nodeBorder, nodeBorder, nodeBorder);
        }

        private void SetupNodeColor(SpeakerType speaker)
        {
            if (speaker == SpeakerType.player)
            {
                nodeStyle.normal.background = EditorGUIUtility.Load("node3") as Texture2D;
            }
            else if (speaker == SpeakerType.speakerOne)
            {
                nodeStyle.normal.background = EditorGUIUtility.Load("node1") as Texture2D;
            }
            else if (speaker == SpeakerType.speakerTwo)
            {
                nodeStyle.normal.background = EditorGUIUtility.Load("node2") as Texture2D;
            }
            else if (speaker == SpeakerType.speakerThree)
            {
                nodeStyle.normal.background = EditorGUIUtility.Load("node5") as Texture2D;
            }
            else if (speaker == SpeakerType.speakerFour)
            {
                nodeStyle.normal.background = EditorGUIUtility.Load("node6") as Texture2D;
            }
            else if (speaker == SpeakerType.speakerMore)
            {
                nodeStyle.normal.background = EditorGUIUtility.Load("node0") as Texture2D;
            }
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
                    selectedDialogue.DeleteNode(deletingNode);
                    deletingNode = null;
                }
                if (creatingNode != null)
                {
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
                selectedNode = null;
                draggable = false;
                draggingOffset = new Vector2();

                Vector2 mousePosition = Event.current.mousePosition;

                selectedNode = GetNodeAtPoint(mousePosition + scrollPosition, out draggable);
                if (selectedNode != null)
                {
                    Selection.activeObject = selectedNode;
                    draggingOffset = selectedNode.GetPosition() - mousePosition;
                }
                else
                {
                    Selection.activeObject = selectedDialogue;
                    draggingOffset = mousePosition + scrollPosition;
                }
            }
            else if (Event.current.type == EventType.MouseDrag && selectedNode != null)
            {
                if (draggable)
                {
                    Vector2 currentMousePosition = Event.current.mousePosition;
                    selectedNode.SetPosition(currentMousePosition + draggingOffset);
                    GUI.changed = true;
                }
            }
            else if (Event.current.type == EventType.MouseDrag && selectedNode == null)
            {
                scrollPosition = draggingOffset - Event.current.mousePosition;
                GUI.changed = true;
            }
            else if (Event.current.type == EventType.MouseUp)
            {
                selectedNode = null;
                draggingOffset = new Vector2();
            }
        }

        private void DrawNode(DialogueNode dialogueNode)
        {
            SetupNodeColor(dialogueNode.GetSpeaker());
            GUILayout.BeginArea(dialogueNode.GetRect(), nodeStyle);

            // Dragging Header
            GUILayout.BeginArea(dialogueNode.GetDraggingRect(), nodeStyle);
            GUILayout.EndArea();

            // Node Properties
            EditorGUIUtility.labelWidth = labelOffset;
            EditorGUILayout.LabelField("Unique ID:", dialogueNode.name);
            EditorGUILayout.Space(nodeBorder);

            // Text Input
            EditorGUILayout.BeginScrollView(Vector2.zero);
            string newText = EditorGUILayout.TextArea(dialogueNode.GetText(), 
                GUILayout.Width(dialogueNode.GetRect().width - nodePadding*2 - nodeBorder*2), 
                GUILayout.Height(textAreaHeight));

            dialogueNode.SetText(newText);
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
            scrollMaxX = Mathf.Max(scrollMaxX, dialogueNode.GetRect().xMax);
            scrollMaxY = Mathf.Max(scrollMaxY, dialogueNode.GetRect().yMax);
        }

        private void DrawAddRemoveButtons(DialogueNode dialogueNode)
        {
            // Set tags to create/delete at end of OnGUI to avoid operating on list while iterating over it
            GUILayout.BeginHorizontal();
            if (!dialogueNode.IsRootNode())
            {
                if (GUILayout.Button("-", GUILayout.Width(dialogueNode.GetRect().width * addRemoveButtonMultiplier)))
                {
                    deletingNode = dialogueNode;
                }
            }

            if (GUILayout.Button("+", GUILayout.Width(dialogueNode.GetRect().width * addRemoveButtonMultiplier)))
            {
                creatingNode = dialogueNode;
            }
            GUILayout.EndHorizontal();
        }

        private void DrawLinkButtons(DialogueNode dialogueNode)
        {
            if (linkingParentNode == null)
            {
                if (GUILayout.Button("link", GUILayout.Width(dialogueNode.GetRect().width * linkButtonMultiplier)))
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

                    if (GUILayout.Button(buttonText, GUILayout.Width(dialogueNode.GetRect().width * linkButtonMultiplier)))
                    {
                        selectedDialogue.ToggleRelation(linkingParentNode, dialogueNode);
                        linkingParentNode = null;
                    }
                }
                else
                {
                    if (GUILayout.Button("---", GUILayout.Width(dialogueNode.GetRect().width * linkButtonMultiplier)))
                    {
                        linkingParentNode = null;
                    }
                }
            }
        }

        private void DrawConnections(DialogueNode dialogueNode)
        {
            Vector2 startPoint = new Vector2(dialogueNode.GetRect().xMax, dialogueNode.GetRect().center.y);
            foreach (DialogueNode childNode in selectedDialogue.GetAllChildren(dialogueNode))
            {
                Vector2 endPoint = new Vector2(childNode.GetRect().xMin, childNode.GetRect().center.y);
                float connectionBezierOffset = (endPoint.x - startPoint.x) * connectionBezierOffsetMultiplier;
                Handles.DrawBezier(startPoint, endPoint, 
                    startPoint + Vector2.right * connectionBezierOffset, endPoint + Vector2.left * connectionBezierOffset, 
                    Color.white, null, connectionBezierWidth);
            }
        }

        private DialogueNode GetNodeAtPoint(Vector2 point, out bool draggable)
        {
            DialogueNode foundNode = null;
            draggable = false;
            foreach (DialogueNode dialogueNode in selectedDialogue.GetAllNodes())
            {
                if (dialogueNode.GetRect().Contains(point))
                {
                    foundNode = dialogueNode;
                }

                Rect draggingRect = new Rect(dialogueNode.GetRect().position, dialogueNode.GetDraggingRect().size);
                if (draggingRect.Contains(point))
                {
                    draggable = true;
                }
            }
            return foundNode;
        }
    }
}