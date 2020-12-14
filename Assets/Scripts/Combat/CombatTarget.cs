using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Attributes;
using RPG.Control;

namespace RPG.Combat
{
    [RequireComponent(typeof(Health))]
    public class CombatTarget : MonoBehaviour, IRaycastable
    {
        public bool HandleRaycast(PlayerController callingController, string interactButtonOne, string interactButtonTwo)
        {
            Fighter fighter = callingController.GetComponent<Fighter>();
            if (fighter == null) { return false; }
            if (!fighter.CanAttack(gameObject)) { return false; }

            if (Input.GetButtonDown(interactButtonOne) || Input.GetButton(interactButtonTwo))
            {
                fighter.Attack(gameObject);
            }
            return true;
        }

        public void HandleDeath()
        {
            GetComponent<Rigidbody>().useGravity = false;
            GetComponent<Collider>().enabled = false;
        }

        public CursorType GetCursorType()
        {
            return CursorType.Combat;
        }
    }
}
