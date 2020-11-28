using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Saving;

namespace RPG.Stats
{
    public class BaseStats : MonoBehaviour, ISaveable
    {
        // Tunables
        [SerializeField][Range(1,99)] int startingLevel = 1;
        [SerializeField] CharacterClass characterClass;
        [SerializeField] Progression progression = null;

        // State
        [SerializeField] int currentLevel = -1;   // REMOVE:  Serialized for debug

        private void Start()
        {
            if (Mathf.Approximately(currentLevel, -1f)) { currentLevel = startingLevel; }
        }

        public float GetHealth()
        {
            return progression.GetHealth(characterClass, currentLevel);
        }

        public object CaptureState()
        {
            return currentLevel;
        }

        public void RestoreState(object state)
        {
            currentLevel = (int)state;
        }
    }
}