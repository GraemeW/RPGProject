using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using RPG.Core;
using RPG.Saving;
using RPG.Stats;
using RPG.Utils;

namespace RPG.Attributes
{
    public class Health : MonoBehaviour, ISaveable
    {
        // Tunables
        [SerializeField] float levelUpHealthFraction = 0.7f;

        // Cached References
        Animator animator = null;
        BaseStats baseStats = null;

        // State
        bool isDead = false;
        float maxHealthPoints = 10f;
        LazyValue<float> currentHealthPoints;

        // Events
        public TakeDamageEvent takeDamage;
        public UnityEvent die;

        [System.Serializable]
        public class TakeDamageEvent : UnityEvent<float>
        {
        }

        private void Awake()
        {
            animator = GetComponent<Animator>();
            baseStats = GetComponent<BaseStats>();

            currentHealthPoints = new LazyValue<float>(GetMaxPoints);
        }

        private void Start()
        {
            currentHealthPoints.ForceInit();
            GetMaxPoints();
        }

        public float GetMaxPoints()
        {
            maxHealthPoints = baseStats.GetStat(Stat.Health);
            return maxHealthPoints;
        }

        private void OnEnable()
        {
            baseStats.OnLevelUp += RestoreHealthOnLevelUp;
        }

        private void OnDisable()
        {
            baseStats.OnLevelUp -= RestoreHealthOnLevelUp;
        }

        public void TakeDamage(GameObject instigator, float damage)
        {
            if (damage > 0)
            { 
                takeDamage.Invoke(damage);
            }

            currentHealthPoints.value = Mathf.Max(currentHealthPoints.value - damage, 0f);
            if (Mathf.Approximately(currentHealthPoints.value, 0f) || currentHealthPoints.value <= 0)
            {
                if (damage > 0) { die.Invoke(); }
                Die();
                AwardExperience(instigator);
            }
        }

        public void Heal(float points)
        {
            if (isDead) { return; }
            currentHealthPoints.value = Mathf.Min(currentHealthPoints.value + points, maxHealthPoints);
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
            experience.GainExperience(baseStats.GetStat(Stat.ExperienceReward));
        }

        public float GetHealthPoints()
        {
            return currentHealthPoints.value;
        }

        public int GetPercentage()
        {
            float healthPercentage = GetFraction() * 100;
            return Mathf.RoundToInt(healthPercentage);
        }

        public float GetFraction()
        {
            float healthFraction = currentHealthPoints.value / maxHealthPoints;
            return healthFraction;
        }

        public bool isMaxHealth()
        {
            return Mathf.Approximately(currentHealthPoints.value, maxHealthPoints);
        }

        public bool IsDead()
        {
            return isDead;
        }

        public void SetDefaultHealth()
        {
            if (baseStats == null) { baseStats = GetComponent<BaseStats>(); }
            maxHealthPoints = baseStats.GetStat(Stat.Health);
        }

        public void RestoreHealthOnLevelUp()
        {
            float currentHealthFraction = currentHealthPoints.value / maxHealthPoints;
            SetDefaultHealth();
            if (currentHealthFraction < levelUpHealthFraction) { currentHealthPoints.value = maxHealthPoints * levelUpHealthFraction; }
            else { currentHealthPoints.value = maxHealthPoints * currentHealthFraction; }
        }

        public object CaptureState()
        {
            return currentHealthPoints.value;
        }

        public void RestoreState(object state)
        {
            currentHealthPoints.value = (float)state;
            TakeDamage(gameObject, 0f);
        }
    }
}

