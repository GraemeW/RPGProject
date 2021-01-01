using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace RPG.Quests
{
    public class QuestStatus
    {
        Quest quest;
        bool[] objectiveStatus;

        public QuestStatus(Quest quest)
        {
            this.quest = quest;
            objectiveStatus = new bool[quest.GetObjectiveCount()];
        }

        public Quest GetQuest()
        {
            return quest;
        }

        public int GetCompletedObjectiveCount()
        {
            return objectiveStatus.Where(c => c).Count();
        }

        public bool GetObjectiveStatusForIndex(int index)
        {
            return objectiveStatus[index];
        }

        public void SetObjective(int index, bool isComplete)
        {
            if (index < 0 || index >= objectiveStatus.Length) { return; }
            objectiveStatus[index] = isComplete;
        }
    }
}