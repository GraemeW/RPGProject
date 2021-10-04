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

        // State
        Dictionary<CharacterClass, Dictionary<Stat, float[]>> lookupTable = null;

        public float GetStat(Stat stat, CharacterClass characterClass, int level)
        {
            BuildLookup();
            if (!lookupTable.ContainsKey(characterClass)) { return 0; }
            if (!lookupTable[characterClass].ContainsKey(stat)) { return 0; }

            float[] levels = lookupTable[characterClass][stat];
            int safeLevel = Mathf.Clamp(level - 1, 0, levels.Length - 1);
            return levels[safeLevel];
        }

        public int GetLevels(Stat stat, CharacterClass characterClass)
        {
            BuildLookup();
            return lookupTable[characterClass][stat].Length;
        }

        private void BuildLookup()
        {
            if (lookupTable != null) { return; }
            lookupTable = new Dictionary<CharacterClass, Dictionary<Stat, float[]>>();
            
            foreach (ProgressionCharacterClass progressionCharacterClass in characterClasses)
            {
                Dictionary<Stat, float[]> statDictionary = new Dictionary<Stat, float[]>();
                foreach (ProgressionStat progressionStat in progressionCharacterClass.stats)
                {
                    statDictionary[progressionStat.stat] = progressionStat.levels;
                }
                lookupTable[progressionCharacterClass.characterClass] = statDictionary;
            }
           
        }

        // Data structures
        [System.Serializable]
        class ProgressionCharacterClass
        {
            public CharacterClass characterClass = CharacterClass.Grunt;
            public ProgressionStat[] stats = default;
        }

        [System.Serializable]
        class ProgressionStat
        {
            public Stat stat = default;
            public float[] levels = default;
        }
    }
}