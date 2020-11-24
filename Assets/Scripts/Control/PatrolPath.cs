using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Control
{
    public class PatrolPath : MonoBehaviour
    {
        // Tunables
        [SerializeField] float waypointGizmoSphereRadius = 0.35f;
        [SerializeField] Color waypointGizmoColor = new Color(1f, 0.5f, 0.5f);
        [SerializeField] Color startSphereColor = new Color(0.2f, 0.8f, 0.2f);
        [SerializeField] float selectAlpha = 0.6f;
        [SerializeField] float deselectAlpha = 0.2f;
        [SerializeField] bool looping = true;
        [SerializeField] bool returnToFirstWaypoint = true;

        // State
        bool loopedOnce = false;
        int incrementDirection = 1;

        private void Start()
        {
            loopedOnce = false;
            incrementDirection = 1;
        }

        private void OnDrawGizmos()
        {
            DrawPatrolPath(deselectAlpha);
        }

        public void OnDrawGizmosSelected()
        {
            DrawPatrolPath(selectAlpha);
        }

        private void DrawPatrolPath(float alphaValue)
        {
            Gizmos.color = new Color(waypointGizmoColor.r, waypointGizmoColor.g, waypointGizmoColor.b, alphaValue);
            for (int waypointIndex = 0; waypointIndex < transform.childCount; waypointIndex++)
            {
                DrawSphere(waypointIndex, alphaValue);
                DrawLine(waypointIndex);
            }
        }

        private void DrawSphere(int waypointIndex, float alphaValue)
        {
            if (waypointIndex == 0) { Gizmos.color = new Color(startSphereColor.r, startSphereColor.g, startSphereColor.b, alphaValue); }

            Gizmos.DrawSphere(GetWaypoint(waypointIndex).position, waypointGizmoSphereRadius);
            if (waypointIndex == 0) { Gizmos.color = new Color(waypointGizmoColor.r, waypointGizmoColor.g, waypointGizmoColor.b, alphaValue); }
        }

        private void DrawLine(int waypointIndex)
        {
            if (transform.childCount == 1) { return; }

            if (waypointIndex != transform.childCount - 1 || returnToFirstWaypoint)
            {
                Gizmos.DrawLine(GetWaypoint(waypointIndex).position, GetWaypoint(GetNextIndex(waypointIndex, true)).position);
            }
        }

        public Transform GetWaypoint(int waypointIndex)
        {
            return transform.GetChild(waypointIndex);
        }

        public int GetNextIndex(int waypointIndex, bool calledFromGizmo = false)
        {
            if (waypointIndex == transform.childCount - 1)
            {
                if (!calledFromGizmo) { loopedOnce = true; }
                if (returnToFirstWaypoint) { return 0; } // end patrol at first index
                else { if (!calledFromGizmo) incrementDirection = -1; } // walk backward
            }
            if (!looping && loopedOnce) { return waypointIndex; } // not looping, end patrol
            if (waypointIndex == 0 && loopedOnce) { if (!calledFromGizmo) incrementDirection = 1; } // walk forward

            if (calledFromGizmo) { return waypointIndex + 1; }
            return waypointIndex + incrementDirection;
        }
    }

}