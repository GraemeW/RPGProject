using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Resources;
using System;

namespace RPG.Stats
{
    public class BaseStats : MonoBehaviour
    {
        // Tunables
        [SerializeField] CharacterClass characterClass = CharacterClass.Grunt;
        [SerializeField] Progression progression = null;
        [Range(1, 99)] [SerializeField] int defaultLevel = 1; // Override if experience class exists

        // State
        int currentLevel = 0;

        // Cached References
        Health health = null;
        Experience experience = null;

        private void Start()
        {
            health = GetComponent<Health>();
            experience = GetComponent<Experience>();
            if (experience != null)
            {
                experience.OnExperienceGained += LevelUp;
            }
            currentLevel = CalculateLevel();
        }

        public float GetStat(Stat stat)
        {
            return progression.GetStat(stat, characterClass, GetLevel());
        }

        public float GetStatForLevel(Stat stat, int level)
        {
            return progression.GetStat(stat, characterClass, level);
        }

        public int GetLevel()
        {
            if (currentLevel < 1) { currentLevel = CalculateLevel(); }
            return currentLevel;
        }

        public void SetLevel()
        {
            currentLevel = CalculateLevel();
        }

        public int CalculateLevel()
        {
            if (experience == null) { experience = GetComponent<Experience>(); }
            if (experience == null) { return defaultLevel; } // Default behavior for enemies

            float currentExperiencePoints = GetComponent<Experience>().GetPoints();
            int penultimateLevel = progression.GetLevels(Stat.ExperienceToLevelUp, characterClass);
            for (int level = 1; level <= penultimateLevel; level++)
            {
                float experienceToLevelUp = progression.GetStat(Stat.ExperienceToLevelUp, characterClass, level);
                if (experienceToLevelUp > currentExperiencePoints)
                {
                    return level;
                }
            }
            return penultimateLevel + 1;
        }

        public void LevelUp()
        {
            int newLevel = CalculateLevel();
            if (newLevel > currentLevel)
            {
                currentLevel = newLevel;
                health.RestoreHealthToMax();
            }
        }
    }
}