using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Dialogue;
using TMPro;
using UnityEngine.UI;
using System;

namespace RPG.UI.Dialogue
{
    public class DialogueUI : MonoBehaviour
    {
        // Tunables
        [Header("Simple Response Options")]
        [SerializeField] TextMeshProUGUI currentSpeaker = null;
        [SerializeField] TextMeshProUGUI dialogueText = null;
        [SerializeField] Button nextButton = null;
        [SerializeField] Color playerNameColor = Color.green;
        [SerializeField] Color otherNameColor = Color.white;
        [Header("Extra Choice Options")]
        [SerializeField] GameObject choiceButton = null;
        [SerializeField] GameObject choiceResponseParent = null;

        // State
        bool panelActive = true; // over-rides on start

        // Cached References
        CanvasGroup canvasGroup = null;
        PlayerConversant playerConversant = null;

        private void Awake()
        {
            canvasGroup = GetComponent<CanvasGroup>();
            playerConversant = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerConversant>();
        }

        private void Start()
        {
            DrawDialoguePanel(false);
        }

        private void OnEnable()
        {
            playerConversant.dialogueUpdated += UpdateUI;
        }

        private void OnDisable()
        {
            playerConversant.dialogueUpdated -= UpdateUI;
        }

        public void Next() // Called by unity event on Next button
        {
            playerConversant.NextRandom();
        }

        public void Choose(string nodeID)
        {
            playerConversant.NextWithID(nodeID);
        }

        public void EndConversation() // Called by unity event on 'x' button
        {
            playerConversant.EndConversation();
        }

        private void DrawDialoguePanel(bool isEnabled)
        {
            if (panelActive != isEnabled)
            {
                float alphaSetting = 0;
                if (isEnabled) { alphaSetting = 1; }

                canvasGroup.alpha = alphaSetting;
                canvasGroup.blocksRaycasts = isEnabled;
                panelActive = isEnabled;
            }
        }

        private void UpdateUI()
        {
            if (!playerConversant.IsActive())
            {
                DrawDialoguePanel(false);
                return;
            }

            DrawDialoguePanel(true);
            ResetUI();
            SetSimpleText();
            if (playerConversant.IsChoosing())
            {
                SetChoiceList();
            }
            else
            {
                nextButton.gameObject.SetActive(playerConversant.HasNext());
            }
            SetSpeakerColor();
        }

        private void ResetUI()
        {
            CleanOldChoices();
            choiceResponseParent.SetActive(false);
            nextButton.gameObject.SetActive(false);
        }

        private void SetSimpleText()
        {
            currentSpeaker.text = playerConversant.GetCurrentSpeakerName();
            dialogueText.text = playerConversant.GetText();
        }

        private void SetChoiceList()
        {
            choiceResponseParent.SetActive(true);
            foreach (DialogueNode choiceNode in playerConversant.GetChoices())
            {
                currentSpeaker.text = playerConversant.GetCurrentSpeakerName();

                GameObject choiceButton = Instantiate(this.choiceButton, choiceResponseParent.transform);
                TextMeshProUGUI choiceText = choiceButton.GetComponentInChildren<TextMeshProUGUI>();
                choiceText.text = choiceNode.GetText();

                choiceButton.GetComponent<Button>().onClick.AddListener(delegate { Choose(choiceNode.name); });
            }
        }

        private void CleanOldChoices()
        {
            foreach (Transform child in choiceResponseParent.transform)
            {
                child.GetComponent<Button>().onClick.RemoveAllListeners();
                Destroy(child.gameObject);
            }
        }

        private void SetSpeakerColor()
        {
            if (playerConversant.GetCurrentSpeaker() == SpeakerType.player)
            {
                currentSpeaker.color = playerNameColor;
            }
            else
            {
                currentSpeaker.color = otherNameColor;
            }
        }
    }
}