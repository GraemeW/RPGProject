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
        [SerializeField] float reviveHealthFraction = 0.3f;

        // Cached References
        Animator animator = null;
        BaseStats baseStats = null;

        // State
        float maxHealthPoints = 10f;
        LazyValue<float> healthPoints;
        bool isDeadLastScene = false;

        // Events
        public TakeDamageEvent takeDamage;
        public UnityEvent onDie;

        [System.Serializable]
        public class TakeDamageEvent : UnityEvent<float>
        {
        }

        private void Awake()
        {
            animator = GetComponent<Animator>();
            baseStats = GetComponent<BaseStats>();

            healthPoints = new LazyValue<float>(GetMaxPoints);
        }

        private void Start()
        {
            healthPoints.ForceInit();
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
            if (!IsDead() && damage > 0)
            { 
                takeDamage.Invoke(damage);
                healthPoints.value = Mathf.Max(healthPoints.value - damage, 0f);
            }

            if (IsDead() && damage > 0)
            {
                // Little hack, but separating this out -- don't want to trigger sounds, respawner, etc.
                // -- e.g. on 0 damage used to trigger animators on scene load from save

                onDie.Invoke();
                AwardExperience(instigator);
            }

            UpdateState();
        }

        public void Heal(float points)
        {
            if (IsDead()) { return; }

            healthPoints.value = Mathf.Min(healthPoints.value + points, maxHealthPoints);
        }

        public void Revive()
        {
            healthPoints.value = GetMaxPoints() * reviveHealthFraction;
            UpdateState();
        }

        private void UpdateState()
        {
            if (!isDeadLastScene && IsDead())
            {
                if (animator == null) { animator = GetComponent<Animator>(); }
                animator.SetTrigger("die");
                GetComponent<ActionScheduler>().CancelCurrentAction();

                isDeadLastScene = true;
            }
            else if (isDeadLastScene && !IsDead())
            {
                animator.Rebind();
            }
        }

        private void AwardExperience(GameObject instigator)
        {
            Experience experience = instigator.GetComponent<Experience>();
            if (experience == null) { return; }
            experience.GainExperience(baseStats.GetStat(Stat.ExperienceReward));
        }

        public float GetHealthPoints()
        {
            return healthPoints.value;
        }

        public int GetPercentage()
        {
            float healthPercentage = GetFraction() * 100;
            return Mathf.RoundToInt(healthPercentage);
        }

        public float GetFraction()
        {
            float healthFraction = healthPoints.value / maxHealthPoints;
            return healthFraction;
        }

        public bool isMaxHealth()
        {
            return Mathf.Approximately(healthPoints.value, maxHealthPoints);
        }

        public bool IsDead()
        {
            return healthPoints.value <= 0;
        }

        public void SetDefaultHealth()
        {
            if (baseStats == null) { baseStats = GetComponent<BaseStats>(); }
            maxHealthPoints = baseStats.GetStat(Stat.Health);
        }

        public void RestoreHealthOnLevelUp()
        {
            float currentHealthFraction = healthPoints.value / maxHealthPoints;
            SetDefaultHealth();
            if (currentHealthFraction < levelUpHealthFraction) { healthPoints.value = maxHealthPoints * levelUpHealthFraction; }
            else { healthPoints.value = maxHealthPoints * currentHealthFraction; }
        }

        public object CaptureState()
        {
            return healthPoints.value;
        }

        public void RestoreState(object state)
        {
            healthPoints.value = (float)state;
            TakeDamage(gameObject, 0f);
        }
    }
}

