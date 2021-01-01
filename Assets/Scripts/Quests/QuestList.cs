using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Quests
{
    public class QuestList : MonoBehaviour
    {
        // Tunables
        List<QuestStatus> questStatuses = new List<QuestStatus>();

        // Events
        public event Action questListUpdated;

        public IEnumerable<QuestStatus> GetQuestStatuses()
        {
            return questStatuses;
        }

        public void AddQuest(Quest quest)
        {
            if (quest == null) { return; }
            if (HasQuest(quest)) { return; }

            QuestStatus newQuestStatus = new QuestStatus(quest);
            questStatuses.Add(newQuestStatus);
            
            if (questListUpdated != null)
            {
                questListUpdated();
            }
        }

        public bool HasQuest(Quest newQuest)
        {
            foreach (QuestStatus questStatus in questStatuses)
            {
                if (questStatus.GetQuest().GetUniqueID() == newQuest.GetUniqueID())
                {
                    return true;
                }
            }
            return false;
        }
    }
}