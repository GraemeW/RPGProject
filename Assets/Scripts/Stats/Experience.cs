using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Resources;
using RPG.Saving;
using RPG.Utils;

namespace RPG.Stats
{
    public class Experience : MonoBehaviour, ISaveable
    {
        // Tunables
        [SerializeField] float initialExperiencePoints = 0f;

        // State
        LazyValue<float> currentPoints;
        LazyValue<float> pointsToNextLevel;

        // Cached references
        BaseStats baseStats = null;
        Health health = null;

        // Events
        public event Action OnExperienceGained;

        private void Awake()
        {
            baseStats = GetComponent<BaseStats>();
            health = GetComponent<Health>();
            currentPoints = new LazyValue<float>(GetInitialPoints);
            pointsToNextLevel = new LazyValue<float>(GetInitialPointsToNextLevel);
        }

        private float GetInitialPoints()
        {
            return initialExperiencePoints;
        }

        private float GetInitialPointsToNextLevel()
        {
            return baseStats.GetStat(Stat.ExperienceToLevelUp);
        }

        private void Start()
        {
            currentPoints.ForceInit();
            pointsToNextLevel.ForceInit();
        }

        public void GainExperience(float points)
        {
            currentPoints.value += points;
            OnExperienceGained();
            pointsToNextLevel.value = baseStats.GetStat(Stat.ExperienceToLevelUp);
        }

        public void OverrideExperience(float points)
        {
            currentPoints.value = points;

            if (baseStats == null) { baseStats = GetComponent<BaseStats>(); }
            baseStats.SetLevel();
            pointsToNextLevel.value = baseStats.GetStat(Stat.ExperienceToLevelUp);

            if (health == null) { health = GetComponent<Health>(); }
            health.SetDefaultHealth();
        }

        public float GetPoints()
        {
            return currentPoints.value;
        }

        public int GetPercentage()
        {
            int lastLevel = baseStats.GetLevel() - 1;
            float pointsToLastLevel = 0f;
            if (lastLevel > 0) { pointsToLastLevel = baseStats.GetStatForLevel(Stat.ExperienceToLevelUp, lastLevel); }
            float healthPercentage = (currentPoints.value - pointsToLastLevel) / (pointsToNextLevel.value - pointsToLastLevel) * 100;
            return Mathf.RoundToInt(healthPercentage);
        }

        public object CaptureState()
        {
            return currentPoints.value;
        }

        public void RestoreState(object state)
        {
            OverrideExperience((float)state);
        }
    }

}