using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Saving;
using RPG.Resources;

namespace RPG.Stats
{
    public class BaseStats : MonoBehaviour
    {
        // Tunables
        [SerializeField][Range(1,99)] int startingLevel = 1;
        [SerializeField] CharacterClass characterClass = CharacterClass.Grunt;
        [SerializeField] Progression progression = null;

        // State
        [SerializeField] int currentLevel = -1;   // REMOVE:  Serialized for debug

        private void Start()
        {
            if (Mathf.Approximately(currentLevel, -1f)) { currentLevel = startingLevel; } // Overridden by load save file
        }

        public float GetStat(Stat stat)
        {
            return progression.GetStat(stat, characterClass, currentLevel);
        }

        public int GetLevel()
        {
            return currentLevel;
        }

        public void SetLevel(int level)
        {
            currentLevel = level;
        }

        public void LevelUp()
        {
            currentLevel++;
        }
    }
}