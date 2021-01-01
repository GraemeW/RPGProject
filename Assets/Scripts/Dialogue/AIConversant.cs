using RPG.Control;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Dialogue
{
    public class AIConversant : MonoBehaviour, IRaycastable
    {
        // Tunables
        [SerializeField] Dialogue dialogue = null;
        [SerializeField] string conversantName = "";

        private void Start()
        {
            if (dialogue != null)
            {
                dialogue.SetSpeaker(SpeakerType.speakerOne, this);
            }
        }

        public string GetConversantName()
        {
            return conversantName;
        }

        public void SetDialogue(Dialogue dialogue)
        {
            if (dialogue == null) { return; }
            this.dialogue = dialogue;
            dialogue.SetSpeaker(SpeakerType.speakerOne, this);
        }

        public bool HandleRaycast(PlayerController callingController, string interactButtonOne = "Fire1", string interactButtonTwo = "Fire2")
        {
            if (dialogue == null) { return false; }

            if (Input.GetButtonDown(interactButtonOne))
            {
                callingController.GetComponent<PlayerConversant>().InitiateConversation(this, dialogue);
            }
            else if (Input.GetButtonDown(interactButtonTwo))
            {
                callingController.InteractWithMovement(true);
            }
            return true;
        }

        public CursorType GetCursorType()
        {
            return CursorType.Dialogue;
        }
    }
}