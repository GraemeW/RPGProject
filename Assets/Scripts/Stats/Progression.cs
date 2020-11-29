using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Stats
{
    [CreateAssetMenu(fileName = "Progression", menuName = "Stats/New Progression", order = 0)]
    public class Progression : ScriptableObject
    {
        // Tunables
        [SerializeField] ProgressionCharacterClass[] characterClasses = default;

        // Functionality

        public float GetStat(Stat stat, CharacterClass characterClass, int level)
        {
            ProgressionStat progressionStat = GetMatchedStat(GetMatchedClass(characterClass), stat);
            float statValue = progressionStat.levels[GetSafeLevel(level, progressionStat)];
            return statValue;
        }

       private ProgressionCharacterClass GetMatchedClass(CharacterClass characterClass)
        {
            foreach (ProgressionCharacterClass progressionCharacterClass in characterClasses)
            {
                if (progressionCharacterClass.characterClass == characterClass)
                {
                    return progressionCharacterClass;
                }
            }
            return default;
        }

        private ProgressionStat GetMatchedStat(ProgressionCharacterClass progressionCharacterClass, Stat stat)
        {
            foreach (ProgressionStat progressionStat in progressionCharacterClass.stats)
            {
                if (progressionStat.stat == stat)
                {
                    return progressionStat;
                }
            }
            return default;
        }

        private int GetSafeLevel(int level, ProgressionStat progressionStat)
        {
            return Mathf.Clamp(level - 1, 0, progressionStat.levels.Length - 1);
        }

        // Data structures
        [System.Serializable]
        class ProgressionCharacterClass
        {
            public CharacterClass characterClass = CharacterClass.Grunt;
            public ProgressionStat[] stats;
        }

        [System.Serializable]
        class ProgressionStat
        {
            public Stat stat;
            public float[] levels;
        }
    }
}