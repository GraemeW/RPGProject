using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Resources;
using RPG.Saving;
using UnityEngine.Events;
using System;

namespace RPG.Stats
{
    public class Experience : MonoBehaviour, ISaveable
    {
        // Tunables
        [SerializeField] float initialExperiencePoints = 0f;

        // State
        float currentPoints = -1f;
        float pointsToNextLevel = Mathf.Infinity;

        // Cached references
        BaseStats baseStats = null;
        Health health = null;

        // Events
        public event Action OnExperienceGained;

        private void Start()
        {
            baseStats = GetComponent<BaseStats>();
            health = GetComponent<Health>();
            if (Mathf.Approximately(currentPoints, -1f)) { currentPoints = initialExperiencePoints; }  // Overridden by load save file
            pointsToNextLevel = baseStats.GetStat(Stat.ExperienceToLevelUp);
        }

        public void GainExperience(float points)
        {
            currentPoints += points;
            OnExperienceGained();
            pointsToNextLevel = baseStats.GetStat(Stat.ExperienceToLevelUp);
        }

        public void OverrideExperience(float points)
        {
            currentPoints = points;

            if (baseStats == null) { baseStats = GetComponent<BaseStats>(); }
            baseStats.SetLevel();
            pointsToNextLevel = baseStats.GetStat(Stat.ExperienceToLevelUp);

            if (health == null) { health = GetComponent<Health>(); }
            health.SetDefaultHealth();
        }

        public float GetPoints()
        {
            return currentPoints;
        }

        public int GetPercentage()
        {
            int lastLevel = baseStats.GetLevel() - 1;
            float pointsToLastLevel = 0f;
            if (lastLevel > 0) { pointsToLastLevel = baseStats.GetStatForLevel(Stat.ExperienceToLevelUp, lastLevel); }
            float healthPercentage = (currentPoints - pointsToLastLevel) / (pointsToNextLevel - pointsToLastLevel) * 100;
            return Mathf.RoundToInt(healthPercentage);
        }

        public object CaptureState()
        {
            return currentPoints;
        }

        public void RestoreState(object state)
        {
            OverrideExperience((float)state);
        }
    }

}