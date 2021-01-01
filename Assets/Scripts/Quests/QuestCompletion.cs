using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Quests
{
    public class QuestCompletion : MonoBehaviour
    {
        [SerializeField] bool active = false;

        // Cached References
        QuestList playerQuestList = null;

        // State
        Quest quest = null;

        // Methods
        private void Awake()
        {
            if (!active) { return; }
            playerQuestList = GameObject.FindGameObjectWithTag("Player").GetComponent<QuestList>();
        }

        public void SetQuest(Quest quest)
        {
            this.quest = quest;
        }

        public void CompleteObjective(int objectiveIndex)
        {
            if (playerQuestList == null) { return; }
            if (quest == null) { return; }

            playerQuestList.CompleteObjective(quest, objectiveIndex);
            quest = null;
        }

        public void ActivateQuestCompleter()
        {
            active = true;
            playerQuestList = GameObject.FindGameObjectWithTag("Player").GetComponent<QuestList>();
        }
    }
}