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
        PlayerConversant playerConversant = null;

        private void Awake()
        {
            playerConversant = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerConversant>();
        }

        private void Start()
        {
            playerConversant.dialogueUpdated += UpdateUI; // Subscribe in Start() because we need to receive messages when disabled
            DrawDialoguePanel(false);
        }

        private void OnDestroy()
        {
            playerConversant.dialogueUpdated -= UpdateUI;
        }

        public void Next() // Called by unity event on Next button
        {
            playerConversant.Next();
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
            if (gameObject.activeSelf != isEnabled)
            {
                gameObject.SetActive(isEnabled);
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
                choiceText.color = playerNameColor;

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