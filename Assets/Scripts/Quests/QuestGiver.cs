using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Quests
{
    public class QuestGiver : MonoBehaviour
    {
        // Tunables
        [SerializeField] QuestTriggerPair[] questTriggerPairs = null;

        // Cached References
        QuestList playerQuestList = null;

        // Data Classes
        [System.Serializable]
        public class QuestTriggerPair
        {
            public string trigger = "";
            public Quest quest = null;
        }

        // Methods
        private void Awake()
        {
            if (questTriggerPairs == null) { return; }
            playerQuestList = GameObject.FindGameObjectWithTag("Player").GetComponent<QuestList>();
        }

        public void GiveQuest(string trigger)
        {
            if (playerQuestList == null) { return; }

            foreach (QuestTriggerPair questTriggerPair in questTriggerPairs)
            {
                if (questTriggerPair.trigger == trigger)
                {
                    playerQuestList.AddQuest(questTriggerPair.quest);
                }
            }
        }
    }
}