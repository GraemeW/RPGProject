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
            if (HasQuest(quest)) { return; }

            QuestStatus newQuestStatus = new QuestStatus(quest);
            questStatuses.Add(newQuestStatus);
            
            if (questListUpdated != null)
            {
                questListUpdated();
            }
        }

        public void CompleteObjective(Quest quest, int objectiveIndex)
        {
            QuestStatus questStatus = GetQuestStatus(quest);
            if (questStatus == null) { return; }

            questStatus.SetObjective(objectiveIndex, true);
            if (questListUpdated != null)
            {
                questListUpdated();
            }
        }

        public bool HasQuest(Quest quest)
        {
            return (GetQuestStatus(quest) != null);
        }

        public QuestStatus GetQuestStatus(Quest quest)
        {
            if (quest == null) { return null; }

            foreach (QuestStatus questStatus in questStatuses)
            {
                if (questStatus.GetQuest().GetUniqueID() == quest.GetUniqueID())
                {
                    return questStatus;
                }
            }
            return null;
        }
    }
}