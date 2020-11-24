using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Core
{
    public class Health : MonoBehaviour
    {
        // Tunables
        [SerializeField] float healthPoints = 100f;

        // Cached References
        Animator animator = null;

        // State
        bool isDead = false;

        private void Start()
        {
            animator = GetComponent<Animator>();
        }

        public void TakeDamage(float damage)
        {
            healthPoints = Mathf.Max(healthPoints - damage, 0f);
            // TODO:  GUI for current health
            UnityEngine.Debug.Log("Health is " + healthPoints);
            if (healthPoints <= 0)
            {
                Die();
            }
        }

        private void Die()
        {
            if (isDead) { return; }

            animator.SetTrigger("die");
            GetComponent<ActionScheduler>().CancelCurrentAction();
            isDead = true;
        }

        public bool IsDead()
        {
            return isDead;
        }
    }
}

