using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Stats
{
    [CreateAssetMenu(fileName = "Progression", menuName = "Stats/New Progression", order = 0)]
    public class Progression : ScriptableObject
    {
        // Tunables
        [SerializeField] ProgressionCharacterClass[] characterClasses;

        // Functionality
        public float GetHealth(CharacterClass characterClass, int level)
        {
            foreach (ProgressionCharacterClass progressionCharacterClass in characterClasses)
            {
                if (progressionCharacterClass.characterClass == characterClass)
                {
                    int levelClamped = Mathf.Clamp(level, 0, progressionCharacterClass.combatPropertiesPerLevel.Length - 1);
                    CombatProperties combatProperties = progressionCharacterClass.combatPropertiesPerLevel[levelClamped];
                    return combatProperties.health;
                }
            }
            return 0f;
        }



        // Data structures
        [System.Serializable]
        class ProgressionCharacterClass
        {
            public CharacterClass characterClass;
            public CombatProperties[] combatPropertiesPerLevel;
        }

        [System.Serializable]
        struct CombatProperties
        {
            public float health;
        }


    }
}