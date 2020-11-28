using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Combat;
using RPG.Core;
using RPG.Movement;
using UnityEngine.Events;
using System;

namespace RPG.Control
{
    public class AIController : MonoBehaviour
    {
        // Tunables
        [Header("Chase Properties")]
        [SerializeField] float chaseDistance = 5.0f;
        [SerializeField] float hostileChaseDistance = 10.0f;
        [SerializeField] float suspicionTime = 3.0f;
        [Header("Patrolling Properties")]
        [SerializeField] PatrolPath patrolPath = null;
        [SerializeField] float waypointTolerance = 1.0f;
        [SerializeField] float waypointDwellTime = 2.0f;
        [Range(0,1)][SerializeField] float patrolSpeedFraction = 0.65f;
        
        // Cached References
        Fighter fighter = null;
        Health health = null;
        Mover mover = null;
        GameObject player = null;

        // States
        Vector3 guardPosition;
        Quaternion guardRotation;
        float timeSinceLastSawPlayer = Mathf.Infinity;
        int currentWaypointIndex = 0;
        float timeSinceArrivedAtWaypoint = Mathf.Infinity;
        float currentChaseDistance = 5.0f;

        private void Start()
        {
            fighter = GetComponent<Fighter>();
            health = GetComponent<Health>();
            mover = GetComponent<Mover>();
            player = GameObject.FindWithTag("Player");

            guardPosition = transform.position;
            guardRotation = transform.rotation;
            currentChaseDistance = chaseDistance;
        }

        private void Update()
        {
            if (health.IsDead()) { return; }

            if (InAttackRange() && fighter.CanAttack(player))
            {
                AttackBehavior();
            }
            else if (timeSinceLastSawPlayer < suspicionTime)
            {
                SuspicionBehavior();
            }
            else
            {
                PatrolBehavior();
            }
            timeSinceLastSawPlayer += Time.deltaTime;
        }

        public void AttackBehavior() // // This is triggered by Health event:  triggeredHostile
        {
            if (health.IsDead()) { return; }
            timeSinceLastSawPlayer = 0f;
            currentChaseDistance = hostileChaseDistance;
            fighter.Attack(player);
        }

        public void CancelAllBehavior()
        {
            fighter.Cancel();
            mover.Cancel();
        }

        private void SuspicionBehavior()
        {
            GetComponent<ActionScheduler>().CancelCurrentAction();
        }

        private void PatrolBehavior()
        {
            Vector3 nextPosition = guardPosition;
            currentChaseDistance = chaseDistance;
            if (patrolPath != null)
            {
                if (AtWaypoint())
                {
                    if (timeSinceArrivedAtWaypoint > waypointDwellTime) { CycleWaypoint(); }
                    timeSinceArrivedAtWaypoint += Time.deltaTime;
                }
                nextPosition = GetCurrentWaypoint();
            }

            mover.StartMoveAction(nextPosition, patrolSpeedFraction);
            if (patrolPath == null) { mover.QueueRotationAfterMove(guardRotation); }
        }

        private Vector3 GetCurrentWaypoint()
        {
            return patrolPath.GetWaypoint(currentWaypointIndex).position;
        }

        private void CycleWaypoint()
        {
            currentWaypointIndex = patrolPath.GetNextIndex(currentWaypointIndex);
            timeSinceArrivedAtWaypoint = 0;
        }

        private bool AtWaypoint()
        {
            float distanceToWaypoint = Vector3.Distance(transform.position, GetCurrentWaypoint());
            return (distanceToWaypoint < waypointTolerance);
        }

        private bool InAttackRange()
        {
            float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
            return (distanceToPlayer < currentChaseDistance);
        }

        // Called by Unity
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, currentChaseDistance);
            if (patrolPath != null) { patrolPath.OnDrawGizmosSelected(); }
        }
    }
}

