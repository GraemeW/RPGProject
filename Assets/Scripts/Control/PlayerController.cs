using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Movement;
using RPG.Combat;
using RPG.Core;
using System;

namespace RPG.Control
{
    public class PlayerController : MonoBehaviour
    {
        // Cached References
        Mover mover = null;
        Fighter fighter = null;
        Health health = null;

        void Start()
        {
            mover = GetComponent<Mover>();
            fighter = GetComponent<Fighter>();
            health = GetComponent<Health>();
        }

        void Update()
        {
            if (health.IsDead()) { return; }

            if (InteractWithCombat()) return;
            if (InteractWithMovement()) return;
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
                    mover.StartMoveAction(hitInfo.point);
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