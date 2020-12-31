using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Combat;
using RPG.Core;
using RPG.Movement;
using RPG.Attributes;
using RPG.Utils;
using System;
using RPG.Saving;

namespace RPG.Control
{
    public class AIController : MonoBehaviour, ISaveable
    {
        // Tunables
        [Header("Disposition")]
        [SerializeField] bool isFriendly = false;
        [Header("Chase Properties")]
        [SerializeField] float chaseDistance = 5.0f;
        [SerializeField] float hostileChaseDistance = 10.0f;
        [SerializeField] float suspicionTime = 3.0f;
        [SerializeField] float aggravationTime = 3.0f;
        [SerializeField] float shoutDistance = 5.0f;
        [Header("Patrolling Properties")]
        [SerializeField] PatrolPath patrolPath = null;
        [SerializeField] float waypointTolerance = 1.0f;
        [SerializeField] float waypointDwellTime = 2.0f;
        [Range(0,1)][SerializeField] float patrolSpeedFraction = 0.65f;
        
        // Cached References
        Fighter fighter = null;
        Health health = null;
        Mover mover = null;
        CombatTarget combatTarget = null;
        GameObject player = null;

        // States
        LazyValue<Vector3> guardPosition;
        LazyValue<Quaternion> guardRotation;
        float timeSinceLastSawPlayer = Mathf.Infinity;
        int currentWaypointIndex = 0;
        float timeSinceArrivedAtWaypoint = Mathf.Infinity;
        LazyValue<float> currentChaseDistance;
        float timeSinceAggravated = Mathf.Infinity;

        private void Awake()
        {
            fighter = GetComponent<Fighter>();
            health = GetComponent<Health>();
            mover = GetComponent<Mover>();
            combatTarget = GetComponent<CombatTarget>();
            player = GameObject.FindWithTag("Player");
            guardPosition = new LazyValue<Vector3>(GetInitialPosition);
            guardRotation = new LazyValue<Quaternion>(GetInitialRotation);
            currentChaseDistance = new LazyValue<float>(GetInitialChaseDistance);
        }

        private Vector3 GetInitialPosition()
        {
            return transform.position;
        }

        private Quaternion GetInitialRotation()
        {
            return transform.rotation;
        }

        private float GetInitialChaseDistance()
        {
            return chaseDistance;
        }

        private void Start()
        {
            guardPosition.ForceInit();
            guardRotation.ForceInit();
            currentChaseDistance.ForceInit();
            if (isFriendly) { SetFriendly(true); }
        }

        private void Update()
        {
            if (health.IsDead()) { return; }
            if (isFriendly) { return; }

            if (IsAggravated() && fighter.CanAttack(player))
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

        public void SetFriendly(bool isFriendly)
        {
            combatTarget.SetActiveTarget(!isFriendly);
            this.isFriendly = isFriendly;
        }

        public void Aggravate()
        {
            if (isFriendly)
            {
                SetFriendly(false);
            }
            timeSinceAggravated = 0f;
        }

        public void AttackBehavior()
        {
            if (health.IsDead()) { return; }
            timeSinceLastSawPlayer = 0f;
            currentChaseDistance.value = hostileChaseDistance;
            fighter.Attack(player);

            AggravateNearbyEnemies();
        }

        private void AggravateNearbyEnemies()
        {
            RaycastHit[] hits = Physics.SphereCastAll(transform.position, shoutDistance, Vector3.up, 0.0f);
            foreach (RaycastHit hit in hits)
            {
                AIController ai = hit.transform.GetComponent<AIController>();
                if (ai == null) { continue; }
                ai.Aggravate();
            }
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
            Vector3 nextPosition = guardPosition.value;
            currentChaseDistance.value = chaseDistance;
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
            if (patrolPath == null) { mover.QueueRotationAfterMove(guardRotation.value); }
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

        private bool IsAggravated()
        {
            float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
            bool inDistance = (distanceToPlayer < currentChaseDistance.value);

            timeSinceAggravated += Time.deltaTime;
            bool aggravated = (timeSinceAggravated < aggravationTime);

            return (inDistance || aggravated);
        }

        // Called by Unity
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, chaseDistance);
            if (patrolPath != null) { patrolPath.OnDrawGizmosSelected(); }
        }

        public object CaptureState()
        {
            return isFriendly;
        }

        public void RestoreState(object state)
        {
            isFriendly = (bool)state;
        }
    }
}

