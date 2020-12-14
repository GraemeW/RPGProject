using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.AI;
using RPG.Movement;
using RPG.Combat;
using RPG.Attributes;

namespace RPG.Control
{
    public class PlayerController : MonoBehaviour
    {
        // Static
        static readonly string LAYER_PICKUP = "Interactable";

        // Data Types
        [System.Serializable]
        public struct CursorMapping
        {
            public CursorType type;
            public Texture2D texture;
            public Vector2 hotspot;
        }

        // Tunables
        [SerializeField] float maxNavMeshProjectedDistance = 1.0f;
        [SerializeField] CursorMapping[] cursorMappings = null;
        [SerializeField] float raycastRadius = 0.6f;

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

            // Click on uninteractable region cancels all actions
            CancelAction();
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
                    if (raycastable.HandleRaycast(this, "Fire2", "Fire1"))
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
            RaycastHit[] hitsInfo = Physics.SphereCastAll(GetMouseRay(), raycastRadius);
            float[] distances = new float[hitsInfo.Length];
            for (int hitIndex = 0; hitIndex < hitsInfo.Length; hitIndex++)
            {
                distances[hitIndex] = hitsInfo[hitIndex].distance;
            }
            Array.Sort(distances, hitsInfo);
            return hitsInfo;
        }

        public bool InteractWithMovement(bool skipMouseClick = false)
        {
            bool hasHit = RaycastNavmesh(out Vector3 target);
            if (hasHit)
            {
                if (!mover.CanMoveTo(target)) { return false; }

                if (Input.GetButton("Fire1") || skipMouseClick)
                {
                    mover.StartMoveAction(target, 1f);
                }
                SetCursor(CursorType.Movement);
                return true;
            }
            return false;
        }

        private bool RaycastNavmesh(out Vector3 target)
        {
            target = new Vector3();
            int ignorePickupLayerMask = ~LayerMask.GetMask(LAYER_PICKUP);
            bool hasHit = Physics.Raycast(GetMouseRay(), out RaycastHit hitInfo, Mathf.Infinity, ignorePickupLayerMask);
            if (!hasHit) { return false; }

            bool hasPosition = NavMesh.SamplePosition(hitInfo.point, out NavMeshHit navMeshHit, maxNavMeshProjectedDistance, NavMesh.AllAreas);
            if (!hasPosition) { return false; }

            target = navMeshHit.position;
            return true;
        }

        private void CancelAction()
        {
            if (Input.GetButtonDown("Fire1"))
            {
                mover.Cancel();
            }
            if (Input.GetButtonDown("Fire2"))
            {
                fighter.Cancel();
            }
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