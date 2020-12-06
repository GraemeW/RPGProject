using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Movement;
using RPG.Combat;
using RPG.Resources;
using System;

namespace RPG.Control
{
    public class PlayerController : MonoBehaviour
    {
        // Cached References
        Mover mover = null;
        Fighter fighter = null;
        Health health = null;

        // State
        public bool isEnabled = true;

        void Awake()
        {
            mover = GetComponent<Mover>();
            fighter = GetComponent<Fighter>();
            health = GetComponent<Health>();
        }

        void Update()
        {
            DropWeapon();
            if (health.IsDead()) { return; }
            if (!isEnabled) { return; }

            if (InteractWithCombat()) return;
            if (InteractWithMovement()) return;
        }

        private void DropWeapon()
        {
            if (Input.GetKeyDown(KeyCode.T))
            {
                fighter.DropWeapon();
            }
        }

        private bool InteractWithCombat()
        {
            RaycastHit[] hitsInfo = Physics.RaycastAll(GetMouseRay());
            foreach (RaycastHit hitInfo in hitsInfo)
            {
                CombatTarget combatTarget = hitInfo.transform.GetComponent<CombatTarget>();
                if (combatTarget == null) { continue; }

                GameObject target = combatTarget.gameObject;
                if (!fighter.CanAttack(target)) { continue; }    

                if (Input.GetButton("Fire1"))
                {
                    fighter.Attack(target);
                }
                return true;
            }
            return false;
        }

        private bool InteractWithMovement()
        {
            bool hasHit = Physics.Raycast(GetMouseRay(), out RaycastHit hitInfo);
            if (hasHit)
            {
                if (Input.GetButton("Fire1"))
                {
                    mover.StartMoveAction(hitInfo.point, 1f);
                }
                return true;
            }
            return false;
        }

        private static Ray GetMouseRay()
        {
            return Camera.main.ScreenPointToRay(Input.mousePosition);
        }
    }
}