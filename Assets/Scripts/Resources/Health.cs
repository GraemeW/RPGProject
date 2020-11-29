using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using RPG.Core;
using RPG.Saving;
using RPG.Stats;

namespace RPG.Resources
{
    public class Health : MonoBehaviour, ISaveable
    {
        // Cached References
        Animator animator = null;
        BaseStats baseStats = null;

        // State
        bool isDead = false;
        float defaultHealthPoints = 10f;
        [SerializeField] float currentHealthPoints = -1f; // REMOVE:  Serialized for debug

        // Events
        public UnityEvent triggeredHostile;

        private void Start()
        {
            animator = GetComponent<Animator>();
            baseStats = GetComponent<BaseStats>();
            defaultHealthPoints = baseStats.GetStat(Stat.health);
            if (Mathf.Approximately(currentHealthPoints, -1f)) { currentHealthPoints = defaultHealthPoints; }  // Overridden by load save file
        }

        public void TakeDamage(GameObject instigator, float damage)
        {
            if (damage > 0) { triggeredHostile.Invoke(); }

            currentHealthPoints = Mathf.Max(currentHealthPoints - damage, 0f);
            // TODO:  GUI for current health
            if (Mathf.Approximately(currentHealthPoints, 0f) || currentHealthPoints <= 0)
            {
                Die();
                AwardExperience(instigator);
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

        private void AwardExperience(GameObject instigator)
        {
            Experience experience = instigator.GetComponent<Experience>();
            if (experience == null) { return; }
            experience.GainExperience(baseStats.GetStat(Stat.experience));
        }

        public int GetPercentage()
        {
            float healthPercentage = currentHealthPoints / defaultHealthPoints * 100;
            return Mathf.RoundToInt(healthPercentage);
        }

        public bool IsDead()
        {
            return isDead;
        }

        public void SetHealthToDefault()
        {
            defaultHealthPoints = baseStats.GetStat(Stat.health);
            currentHealthPoints = defaultHealthPoints;
        }

        public object CaptureState()
        {
            return currentHealthPoints;
        }

        public void RestoreState(object state)
        {
            currentHealthPoints = (float)state;
            TakeDamage(gameObject, 0f);
        }
    }
}

