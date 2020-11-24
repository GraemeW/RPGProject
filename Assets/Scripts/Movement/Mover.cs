using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using RPG.Core;

namespace RPG.Movement
{
    public class Mover : MonoBehaviour, IAction
    {
        // Tunables
        [SerializeField] float pathEndThreshold = 0.1f;
        [SerializeField] float maxSpeed = 5.66f;
        [SerializeField] float rotationSpeed = 5.0f;

        // Cached References
        NavMeshAgent navMeshAgent = null;
        Animator animator = null;
        ActionScheduler actionScheduler = null;
        Health health = null;

        // State
        bool hasPath = false;
        bool rotationQueuedOnPathEnd = false;
        Quaternion queuedRotation;

        private void Start()
        {
            navMeshAgent = GetComponent<NavMeshAgent>();
            animator = GetComponent<Animator>();
            actionScheduler = GetComponent<ActionScheduler>();
            health = GetComponent<Health>();
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

        public void MoveTo(Vector3 destination, float speedFraction)
        {
            navMeshAgent.isStopped = false;
            SetSpeed(speedFraction);
            navMeshAgent.SetDestination(destination);
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
            navMeshAgent.isStopped = true;
        }

        private void SetSpeed(float speedFraction)
        {
            navMeshAgent.speed = Mathf.Clamp01(speedFraction) * maxSpeed;
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
    }
}
