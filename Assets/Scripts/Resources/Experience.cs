using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Stats;
using RPG.Saving;

namespace RPG.Resources
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

        private void Start()
        {
            baseStats = GetComponent<BaseStats>();
            health = GetComponent<Health>();
            if (Mathf.Approximately(currentPoints, -1f)) { currentPoints = initialExperiencePoints; }  // Overridden by load save file
            pointsToNextLevel = baseStats.GetStat(Stat.experience);
        }

        public void GainExperience(float points)
        {
            currentPoints += points;
            if (currentPoints >= pointsToNextLevel)
            {
                LevelUp();
            }
        }

        public void OverrideExperience(float points)
        {
            currentPoints = points;
            if (baseStats == null) { baseStats = GetComponent<BaseStats>(); }
            pointsToNextLevel = baseStats.GetStat(Stat.experience);
        }

        public float GetPoints()
        {
            return currentPoints;
        }

        public int GetPercentage()
        {
            float healthPercentage = currentPoints / pointsToNextLevel * 100;
            return Mathf.RoundToInt(healthPercentage);
        }

        private void LevelUp()
        {
            baseStats.LevelUp();
            health.SetHealthToDefault();
            currentPoints = 0;
            pointsToNextLevel = baseStats.GetStat(Stat.experience);
        }


        [System.Serializable]
        struct LevelState
        {
            public int level;
            public float experiencePoints;
        }
        public object CaptureState()
        {
            if (baseStats == null) { baseStats = GetComponent<BaseStats>(); }
            LevelState levelState = new LevelState
            {
                level = baseStats.GetLevel(),
                experiencePoints = currentPoints
            };

            return levelState;
        }

        public void RestoreState(object state)
        {
            LevelState levelState = (LevelState)state;
            if (baseStats == null) { baseStats = GetComponent<BaseStats>(); }
            baseStats.SetLevel(levelState.level);
            OverrideExperience(levelState.experiencePoints);
        }
    }

}