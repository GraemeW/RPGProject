using UnityEngine;
using RPG.Inventories;
using RPG.Movement;

namespace RPG.Control
{
    [RequireComponent(typeof(Pickup))]
    public class PickupClickable : MonoBehaviour, IRaycastable
    {
        Pickup pickup = null;

        private void Awake()
        {
            pickup = GetComponent<Pickup>();
        }

        public CursorType GetCursorType()
        {
            if (pickup.CanBePickedUp())
            {
                return CursorType.Pickup;
            }
            else
            {
                return CursorType.FullPickup;
            }
        }

        public bool HandleRaycast(PlayerController callingController, string interactButtonOne = "Fire1", string interactButtonTwo = "Fire2")
        {
            Mover callingControllerMover = callingController.GetComponent<Mover>();
            if (!callingControllerMover.CanMoveTo(transform.position)) { return false; }

            if (Input.GetButtonDown(interactButtonOne))
            {
                pickup.PickupItem();
            }
            else if (Input.GetButtonDown(interactButtonTwo))
            {
                callingController.InteractWithMovement(true);
            }
            return true;
        }
    }
}