using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Quests
{
    public class QuestGiver : MonoBehaviour
    {
        // Tunables
        [SerializeField] bool active = false;

        // Cached References
        QuestList playerQuestList = null;

        // Methods
        private void Awake()
        {
            if (!active) { return; }
            playerQuestList = GameObject.FindGameObjectWithTag("Player").GetComponent<QuestList>();
        }

        public void GiveQuest(Quest quest)
        {
            if (playerQuestList == null) { return; }
            playerQuestList.AddQuest(quest);
        }

        public void ActivateQuestGiver()
        {
            active = true;
            playerQuestList = GameObject.FindGameObjectWithTag("Player").GetComponent<QuestList>();
        }
    }
}