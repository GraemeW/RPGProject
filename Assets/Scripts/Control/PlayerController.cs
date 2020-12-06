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
        // Data Types
        enum CursorType
        {
            None,
            Movement,
            Combat
        }

        [System.Serializable]
        struct CursorMapping
        {
            public CursorType type;
            public Texture2D texture;
            public Vector2 hotspot;
        }

        // Tunables
        [SerializeField] CursorMapping[] cursorMappings = null;

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
            SetCursor(CursorType.None);
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
                SetCursor(CursorType.Combat);
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
                SetCursor(CursorType.Movement);
                return true;
            }
            return false;
        }

        private void SetCursor(CursorType type)
        {
            CursorMapping mapping = GetCursorMapping(type);
            Cursor.SetCursor(mapping.texture, mapping.hotspot, CursorMode.Auto);
        }

        private CursorMapping GetCursorMapping(CursorType type)
        {
            foreach (CursorMapping cursorMapping in cursorMappings)
            {
                if (cursorMapping.type == type)
                {
                    return cursorMapping;
                }
            }
            return cursorMappings[0];
        }

        private static Ray GetMouseRay()
        {
            return Camera.main.ScreenPointToRay(Input.mousePosition);
        }
    }
}