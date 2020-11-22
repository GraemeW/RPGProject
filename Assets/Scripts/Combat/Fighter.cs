using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Movement;
using RPG.Core;

namespace RPG.Combat
{
    public class Fighter : MonoBehaviour, IAction
    {
        // Tunables
        [SerializeField] float weaponRange = 2.0f;
        [SerializeField] float timeBetweenAttacks = 1.5f;
        [SerializeField] float weaponDamage = 5.0f;

        // Cached References
        Mover mover = null;
        ActionScheduler actionScheduler = null;
        Animator animator = null;

        // State
        public Transform target = null;
        float timeSinceLastAttack = 0f;

        private void Start()
        {
            mover = GetComponent<Mover>();
            actionScheduler = GetComponent<ActionScheduler>();
            animator = GetComponent<Animator>();
        }

        private void Update()
        {
            timeSinceLastAttack += Time.deltaTime;
            if (target == null) { return; }

            bool inRange = GetIsInRange();
            if (inRange)
            {
                mover.Cancel();
                AttackBehavior();
            }
            else
            {
                mover.MoveTo(target.position);
            }
        }

        private void AttackBehavior()
        {
            if (timeSinceLastAttack > timeBetweenAttacks)
            {
                // This will trigger a Hit() event
                animator.SetTrigger("attack");
                timeSinceLastAttack = 0f;
            }
        }

        private bool GetIsInRange()
        {
            return Vector3.Distance(transform.position, target.transform.position) < weaponRange;
        }

        public void Attack(CombatTarget target)
        {
            actionScheduler.StartAction(this);
            this.target = target.transform;
        }

        public void Cancel()
        {
            target = null;
        }

        // Animation Event
        public void Hit()
        {
            if (target == null) { return; }

            Health targetHealth = target.GetComponent<Health>();
            if (targetHealth != null)
            {
                targetHealth.TakeDamage(weaponDamage);
            }
        }
    }
}
