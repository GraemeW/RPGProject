using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Utils;

namespace RPG.Stats
{
    public class BaseStats : MonoBehaviour
    {
        // Tunables
        [SerializeField] CharacterClass characterClass = CharacterClass.Grunt;
        [SerializeField] Progression progression = null;
        [Range(1, 99)] [SerializeField] int defaultLevel = 1; // Override if experience class exists
        [SerializeField] GameObject levelUpVFXPrefab = null;
        [SerializeField] bool shouldUseModifiers = false;

        // State
        LazyValue<int> currentLevel;

        // Cached References
        Experience experience = null;

        // Events
        public event Action OnLevelUp;

        private void Awake()
        {
            experience = GetComponent<Experience>();
            currentLevel = new LazyValue<int>(CalculateLevel);
        }

        private void Start()
        {
            currentLevel.ForceInit();
        }

        private void OnEnable()
        {
            if (experience != null)
            {
                experience.OnExperienceGained += UpdateLevel;
            }
        }

        private void OnDisable()
        {
            if (experience != null)
            {
                experience.OnExperienceGained -= UpdateLevel;
            }
        }

        public float GetStat(Stat stat)
        {
            return (GetBaseStat(stat) + GetAdditiveModifier(stat)) * (1 + GetPercentageModifier(stat) / 100f);
        }

        public float GetSummedBaseStat(Stat stat)
        {
            return progression.GetSummedStat(stat, characterClass, GetLevel());
        }

        private float GetBaseStat(Stat stat)
        {
            return progression.GetStat(stat, characterClass, GetLevel());
        }

        private float GetAdditiveModifier(Stat stat)
        {
            if (!shouldUseModifiers) return 0f;

            float sumModifier = 0f;
            IModifierProvider[] modifierProviders = GetComponents<IModifierProvider>();
            foreach (IModifierProvider modifierProvider in modifierProviders)
            {
                foreach (float modifier in modifierProvider.GetAdditiveModifiers(stat))
                {
                    sumModifier += modifier;
                }
            }
            return sumModifier;
        }

        private float GetPercentageModifier(Stat stat)
        {
            if (!shouldUseModifiers) return 0f;

            float percentageModifier = 0f;
            IModifierProvider[] modifierProviders = GetComponents<IModifierProvider>();
            foreach (IModifierProvider modifierProvider in modifierProviders)
            {
                foreach (float modifier in modifierProvider.GetPercentageModifiers(stat))
                {
                    percentageModifier += modifier;
                }
            }
            return percentageModifier;
        }

        public float GetStatForLevel(Stat stat, int level)
        {
            return progression.GetStat(stat, characterClass, level);
        }

        public float GetNumberOfLevels()
        {
            return progression.GetLevels(Stat.ExperienceToLevelUp, characterClass);
        }

        public int GetLevel()
        {
            return currentLevel.value;
        }

        public void SetLevel()
        {
            currentLevel.value = CalculateLevel();
        }

        private int CalculateLevel()
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

        public void UpdateLevel()
        {
            int newLevel = CalculateLevel();
            if (newLevel > currentLevel.value)
            {
                currentLevel.value = newLevel;
                OnLevelUp();
                if (levelUpVFXPrefab != null)
                {
                    GameObject levelUpVFX = Instantiate(levelUpVFXPrefab, transform.position, Quaternion.identity);
                    levelUpVFX.transform.parent = transform;
                }
            }
        }
    }
}