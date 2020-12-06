using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Movement;
using RPG.Combat;
using RPG.Resources;
using System;
using UnityEngine.EventSystems;

namespace RPG.Control
{
    public class PlayerController : MonoBehaviour
    {
        // Data Types
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
            // Special handling
            if (InteractWithUI()) return;
            if (health.IsDead()) 
            {
                SetCursor(CursorType.None);
                return; 
            }
            if (!isEnabled) { return; }
            
            // Actions w/out return
            DropWeapon();

            // Actions w/ return
            if (InteractWithComponent()) return;
            if (InteractWithMovement()) return;
            SetCursor(CursorType.None);
        }

        private bool InteractWithUI()
        {
            if (EventSystem.current.IsPointerOverGameObject()) // returns bool on if over UI element
            {
                SetCursor(CursorType.UI);
                return true;
            }
            return false;
        }

        private void DropWeapon()
        {
            if (Input.GetKeyDown(KeyCode.T))
            {
                fighter.DropWeapon();
            }
        }

        private bool InteractWithComponent()
        {
            RaycastHit[] hitsInfo = RaycastAllSorted();
            foreach (RaycastHit hitInfo in hitsInfo)
            {
                IRaycastable[] raycastables = hitInfo.transform.GetComponents<IRaycastable>();
                foreach (IRaycastable raycastable in raycastables)
                {
                    if (raycastable.HandleRaycast(this, "Fire2"))
                    {
                        SetCursor(raycastable.GetCursorType());
                        return true;
                    }
                }
            }
            return false;
        }

        RaycastHit[] RaycastAllSorted()
        {
            RaycastHit[] hitsInfo = Physics.RaycastAll(GetMouseRay());
            float[] distances = new float[hitsInfo.Length];
            for (int hitIndex = 0; hitIndex < hitsInfo.Length; hitIndex++)
            {
                distances[hitIndex] = hitsInfo[hitIndex].distance;
            }
            Array.Sort(distances, hitsInfo);
            return hitsInfo;
        }

        public bool InteractWithMovement()
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