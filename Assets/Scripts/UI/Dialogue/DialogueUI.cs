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
            ClearDialoguePanel();
        }

        private void OnEnable()
        {
            playerConversant.dialogueInitiated += DrawDialoguePanel;
            playerConversant.dialogueUpdated += UpdateUI;
            playerConversant.dialogueEnded += ClearDialoguePanel;
        }

        private void OnDisable()
        {
            playerConversant.dialogueInitiated -= DrawDialoguePanel;
            playerConversant.dialogueUpdated -= UpdateUI;
            playerConversant.dialogueEnded -= ClearDialoguePanel;
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

        private void DrawDialoguePanel()
        {
            canvasGroup.alpha = 1;
            canvasGroup.blocksRaycasts = true;
            UpdateUI();
        }

        private void UpdateUI()
        {
            ResetUI();

            SetSimpleTextDialogue();
            if (playerConversant.GetChoiceCount() > 1 && playerConversant.GetNextSpeaker() == SpeakerType.player)
            {
                SetChoiceDialogue();
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

        private void SetSimpleTextDialogue()
        {
            currentSpeaker.text = playerConversant.GetCurrentSpeakerName();
            dialogueText.text = playerConversant.GetText();
        }

        private void SetChoiceDialogue()
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

        private void ClearDialoguePanel()
        {
            canvasGroup.alpha = 0;
            canvasGroup.blocksRaycasts = false;
        }
    }
}