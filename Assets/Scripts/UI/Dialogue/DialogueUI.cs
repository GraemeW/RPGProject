using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Dialogue;
using TMPro;

namespace RPG.UI.Dialogue
{
    public class DialogueUI : MonoBehaviour
    {
        // Tunables
        [SerializeField] TextMeshProUGUI currentSpeaker = null;
        [SerializeField] TextMeshProUGUI dialogueText = null;
        [SerializeField] TextMeshProUGUI nextButtonText = null;

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
            playerConversant.dialogueUpdated += UpdateDialogue;
            playerConversant.dialogueEnded += ClearDialoguePanel;
        }

        private void OnDisable()
        {
            playerConversant.dialogueInitiated -= DrawDialoguePanel;
            playerConversant.dialogueEnded -= ClearDialoguePanel;
        }

        private void DrawDialoguePanel()
        {
            canvasGroup.alpha = 1;
            canvasGroup.blocksRaycasts = true;
            UpdateDialogue();
        }

        private void UpdateDialogue()
        {
            currentSpeaker.text = playerConversant.GetCurrentSpeakerName();
            dialogueText.text = playerConversant.GetText();
        }

        private void ClearDialoguePanel()
        {
            canvasGroup.alpha = 0;
            canvasGroup.blocksRaycasts = false;
        }
    }
}