using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Stats;
using RPG.Saving;

namespace RPG.Resources
{
    public class Experience : MonoBehaviour
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
            pointsToNextLevel = baseStats.GetExperience();
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
            pointsToNextLevel = baseStats.GetExperience();
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
            pointsToNextLevel = baseStats.GetExperience();
        }
    }

}