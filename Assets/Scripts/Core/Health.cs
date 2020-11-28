using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using RPG.Saving;

namespace RPG.Core
{
    public class Health : MonoBehaviour, ISaveable
    {
        // Tunables
        [SerializeField] float healthPoints = 100f;

        // Cached References
        Animator animator = null;

        // State
        bool isDead = false;

        // Events
        public UnityEvent triggeredHostile;

        private void Start()
        {
            animator = GetComponent<Animator>();
        }

        public void TakeDamage(float damage)
        {
            if (damage > 0) { triggeredHostile.Invoke(); }

            healthPoints = Mathf.Max(healthPoints - damage, 0f);
            // TODO:  GUI for current health
            if (Mathf.Approximately(healthPoints, 0f) || healthPoints <= 0)
            {
                Die();
            }
        }

        private void Die()
        {
            if (isDead) { return; }

            if (animator == null) { animator = GetComponent<Animator>(); }
            animator.SetTrigger("die");
            GetComponent<ActionScheduler>().CancelCurrentAction();
            isDead = true;
        }

        public bool IsDead()
        {
            return isDead;
        }

        public object CaptureState()
        {
            return healthPoints;
        }

        public void RestoreState(object state)
        {
            healthPoints = (float)state;
            TakeDamage(0f);
        }
    }
}

