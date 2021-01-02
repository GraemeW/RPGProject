using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

namespace RPG.Quests
{
    public class QuestStatus
    {
        // State
        Quest quest;
        List<string> completedObjectives = new List<string>();

        // Data structures
        [System.Serializable]
        public struct SerializableQuestStatus
        {
            public string questID;
            public List<string> completedObjectives;
        }

        // Methods
        public QuestStatus(Quest quest)
        {
            this.quest = quest;
        }

        public QuestStatus(SerializableQuestStatus restoreState)
        {
            quest = Quest.GetFromID(restoreState.questID);
            completedObjectives = restoreState.completedObjectives;
        }

        public Quest GetQuest()
        {
            return quest;
        }

        public int GetCompletedObjectiveCount()
        {
            return completedObjectives.Count;
        }

        public bool GetStatusForObjectiveID(string objectiveID)
        {
            return completedObjectives.Contains(objectiveID);
        }

        public void SetObjective(string objectiveID, bool isComplete)
        {
            if (!quest.HasObjective(objectiveID)) { return; }

            if (isComplete && !completedObjectives.Contains(objectiveID))
            {
                completedObjectives.Add(objectiveID);
            }

            if (!isComplete && completedObjectives.Contains(objectiveID))
            {
                completedObjectives.Remove(objectiveID);
            }
        }

        public bool IsComplete()
        {
            return (completedObjectives.Count >= quest.GetObjectiveCount());
        }

        public SerializableQuestStatus CaptureState()
        {
            SerializableQuestStatus serializableQuestStatus = new SerializableQuestStatus();
            serializableQuestStatus.questID = quest.GetUniqueID();
            serializableQuestStatus.completedObjectives = completedObjectives;
            return serializableQuestStatus;
        }
    }
}