using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.AI;
using RPG.Movement;
using RPG.Combat;
using RPG.Attributes;
using RPG.Inventories;

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
        [Header("Interact Types")]
        string movementInteract = "Fire1";
        string componentInteract = "Fire2";
        string uiInteract = "Fire1";

        // Cached References
        Mover mover = null;
        Fighter fighter = null;
        Health health = null;

        // State
        public bool isEnabled = true;
        bool isInteractingWithUI = false;

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
            CheckSpecialAbilityKeys();

            // Actions
            if (InteractWithComponent()) return;
            if (InteractWithMovement()) return;
            SetCursor(CursorType.None);

            // Click on uninteractable region cancels all actions
            CancelAction();
        }

        private bool InteractWithUI()
        {
            if (Input.GetButtonUp(uiInteract)) { isInteractingWithUI = false; }

            if (EventSystem.current.IsPointerOverGameObject()) // returns bool on if over UI element
            {
                SetCursor(CursorType.UI);
                if (Input.GetButtonDown(uiInteract))
                {
                    isInteractingWithUI = true;
                }
                return true;
            }

            if (isInteractingWithUI) { return true; }
            return false;
        }

        private bool InteractWithComponent()
        {
            RaycastHit[] hitsInfo = RaycastAllSorted();
            foreach (RaycastHit hitInfo in hitsInfo)
            {
                IRaycastable[] raycastables = hitInfo.transform.GetComponents<IRaycastable>();
                foreach (IRaycastable raycastable in raycastables)
                {
                    if (raycastable.HandleRaycast(this, componentInteract, movementInteract))
                    {
                        SetCursor(raycastable.GetCursorType());
                        return true;
                    }
                }
            }
            return false;
        }

        private void CheckSpecialAbilityKeys()
        {
            var actionStore = GetComponent<ActionStore>();
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                actionStore.Use(0, gameObject);
            }
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                actionStore.Use(1, gameObject);
            }
            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                actionStore.Use(2, gameObject);
            }
            if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                actionStore.Use(3, gameObject);
            }
            if (Input.GetKeyDown(KeyCode.Alpha5))
            {
                actionStore.Use(4, gameObject);
            }
            if (Input.GetKeyDown(KeyCode.Alpha6))
            {
                actionStore.Use(5, gameObject);
            }
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

                if (Input.GetButton(movementInteract) || skipMouseClick)
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
            if (Input.GetButtonDown(movementInteract))
            {
                mover.Cancel();
            }
            if (Input.GetButtonDown(componentInteract))
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