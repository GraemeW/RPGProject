using RPG.Control;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace RPG.Core
{
    public class WorldTrigger : MonoBehaviour, IRaycastable
    {
        // Tunables
        [SerializeField] UnityEvent onTrigger;

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                onTrigger.Invoke();
                gameObject.SetActive(false);
            }
        }

        public CursorType GetCursorType()
        {
            return CursorType.Movement;
        }

        public bool HandleRaycast(PlayerController callingController, string interactButtonOne = "Fire1", string interactButtonTwo = "Fire2")
        {
            if (Input.GetButtonDown(interactButtonOne) || Input.GetButtonDown(interactButtonTwo))
            {
                callingController.InteractWithMovement(true);
            }
            return true;
        }
    }
}