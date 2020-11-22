using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Combat
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
                QueueDeathSequence();
            }
        }

        private void QueueDeathSequence()
        {
            if (isDead) { return; }

            animator.SetTrigger("die");
            isDead = true;
        }

        public bool IsDead()
        {
            return isDead;
        }
    }
}

