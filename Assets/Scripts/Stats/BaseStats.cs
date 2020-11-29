using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Saving;
using RPG.Resources;

namespace RPG.Stats
{
    public class BaseStats : MonoBehaviour, ISaveable
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

        public float GetHealth()
        {
            return progression.GetStat(Stat.health, characterClass, currentLevel);
        }

        public float GetExperience()
        {
            return progression.GetStat(Stat.experience, characterClass, currentLevel);
        }

        public int GetLevel()
        {
            return currentLevel;
        }

        public void LevelUp()
        {
            currentLevel++;
        }


        [System.Serializable]
        struct LevelState
        {
            public int level;
            public float experiencePoints;
        }

        public object CaptureState()
        {
            // Fold in experience and level on the same state to avoid race condition
            float points = 0f;
            Experience experience = GetComponent<Experience>();
            if (experience != null) { points = experience.GetPoints(); }

            LevelState levelState = new LevelState
            {
                level = currentLevel,
                experiencePoints = points
            };

            return levelState;
        }

        public void RestoreState(object state)
        {
            LevelState levelState = (LevelState)state;
            currentLevel = levelState.level;

            Experience experience = GetComponent<Experience>();
            if (experience != null) 
            {
                experience.OverrideExperience(levelState.experiencePoints);
            }
        }
    }
}