using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Quests
{
    public class QuestCompletion : MonoBehaviour
    {
        // Called via Unity event via DialogueTriggers

        // Tunables
        [SerializeField] bool active = false;
        [SerializeField] Quest quest = null;

        // Cached References
        QuestList playerQuestList = null;

        // Static Methods
        public static void CompletePlayerObjective(string questID, string objectiveID)
        {
            Quest quest = Quest.GetFromID(questID);
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player == null) { return; }

            player.GetComponent<QuestList>().CompleteObjective(quest, objectiveID);
        }    

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

        public void CompleteObjective(string objectiveID)
        {
            if (playerQuestList == null) { return; }
            if (quest == null) { return; }

            playerQuestList.CompleteObjective(quest, objectiveID);
        }

        public void ActivateQuestCompleter()
        {
            active = true;
            playerQuestList = GameObject.FindGameObjectWithTag("Player").GetComponent<QuestList>();
        }
    }
}