using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using RPG.Core;
using RPG.Saving;
using RPG.Attributes;
using RPG.Stats;

namespace RPG.Movement
{
    public class Mover : MonoBehaviour, IAction, ISaveable
    {
        // Tunables
        [SerializeField] float pathEndThreshold = 0.1f;
        [SerializeField] float maxNavPathLength = 40f;
        [SerializeField] float maxSpeed = 5.66f;
        [SerializeField] float rotationSpeed = 5.0f;

        // Cached References
        NavMeshAgent navMeshAgent = null;
        Animator animator = null;
        ActionScheduler actionScheduler = null;
        Health health = null;
        BaseStats baseStats = null;

        // State
        bool hasPath = false;
        bool rotationQueuedOnPathEnd = false;
        Quaternion queuedRotation;

        private void Awake()
        {
            navMeshAgent = GetComponent<NavMeshAgent>();
            animator = GetComponent<Animator>();
            actionScheduler = GetComponent<ActionScheduler>();
            health = GetComponent<Health>();
            baseStats = GetComponent<BaseStats>();
        }

        private void Update()
        {
            navMeshAgent.enabled = !health.IsDead();
            UpdateAnimator();

            if (health.IsDead()) { return; }
            CheckIfReachedDestination();
            RotateOnPathEnd();
        }

        public void StartMoveAction(Vector3 destination, float speedFraction)
        {
            actionScheduler.StartAction(this);
            MoveTo(destination, speedFraction);
        }

        public bool CanMoveTo(Vector3 destination)
        {
            NavMeshPath path = new NavMeshPath();
            bool hasPath = NavMesh.CalculatePath(transform.position, destination, NavMesh.AllAreas, path);

            if (!hasPath) { return false; }
            if (!(path.status == NavMeshPathStatus.PathComplete)) { return false; }
            if (GetPathLength(path) > maxNavPathLength) { return false; }

            return true;
        }

        public void MoveTo(Vector3 destination, float speedFraction)
        {
            navMeshAgent.isStopped = false;
            SetSpeed(speedFraction);
            navMeshAgent.SetDestination(destination);
        }

        private float GetMoveSpeed()
        {
            return (1f + baseStats.GetStat(Stat.MoveSpeed));
        }

        public void QueueRotationAfterMove(Quaternion rotation)
        {
            rotationQueuedOnPathEnd = true;
            queuedRotation = rotation;
        }

        public void Cancel()
        {
            hasPath = false;
            rotationQueuedOnPathEnd = false;
            if (!navMeshAgent.enabled) { return; }
            navMeshAgent.isStopped = true;
        }

        private float GetPathLength(NavMeshPath path)
        {
            float totalPathDistance = 0f;
            if (path.corners.Length < 2) return totalPathDistance;
            for (int cornerIndex = 1; cornerIndex < path.corners.Length; cornerIndex++) //index off of 1 to avoid underrun
            {
                totalPathDistance += Vector3.Distance(path.corners[cornerIndex - 1], path.corners[cornerIndex]);
            }
            return totalPathDistance;
        }

        private void SetSpeed(float speedFraction)
        {
            navMeshAgent.speed = Mathf.Clamp01(speedFraction) * maxSpeed * GetMoveSpeed();
        }

        private void UpdateAnimator()
        {
            Vector3 velocity = navMeshAgent.velocity;
            Vector3 localVelocity = transform.InverseTransformDirection(velocity);
            animator.SetFloat("forwardSpeed", localVelocity.z);
        }

        private void RotateSmoothly()
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, queuedRotation, Time.deltaTime * rotationSpeed);
        }

        private void CheckIfReachedDestination()
        {
            hasPath |= navMeshAgent.hasPath;
            if (hasPath && navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance + pathEndThreshold)
            {
                hasPath = false;
            }
        }

        private void RotateOnPathEnd()
        {
            if (rotationQueuedOnPathEnd && !hasPath)
            {
                RotateSmoothly();
                if (Mathf.Approximately(Quaternion.Angle(transform.rotation, queuedRotation), 0f))
                {
                    rotationQueuedOnPathEnd = false;
                }
            }
        }

        public void TeleportToPosition(Vector3 position, Quaternion rotation)
        {
            if (navMeshAgent == null) { navMeshAgent = GetComponent<NavMeshAgent>(); }
            navMeshAgent.Warp(position);
            transform.rotation = rotation;
        }

        [System.Serializable] struct MoverSaveData
        {
            public SerializableVector3 position;
            public SerializableVector3 rotation;
        }

        public object CaptureState()
        {
            MoverSaveData data = new MoverSaveData
            {
                position = new SerializableVector3(transform.position),
                rotation = new SerializableVector3(transform.rotation.eulerAngles)
            };
            return data;
        }

        public void RestoreState(object state)
        {
            if (navMeshAgent == null) { navMeshAgent = GetComponent<NavMeshAgent>(); }

            MoverSaveData data = (MoverSaveData)state;
            navMeshAgent.Warp(data.position.ToVector());
            transform.rotation = Quaternion.Euler(data.rotation.ToVector());
        }
    }
}
