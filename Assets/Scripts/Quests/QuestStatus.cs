using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace RPG.Quests
{
    public class QuestStatus
    {
        Quest quest;
        bool[] completedObjective;

        public QuestStatus(Quest quest)
        {
            this.quest = quest;
            completedObjective = new bool[quest.GetObjectiveCount()];
        }

        public Quest GetQuest()
        {
            return quest;
        }

        public int GetCompletedObjectiveCount()
        {
            return completedObjective.Where(c => c).Count();
        }

        public bool GetObjectiveStatusForIndex(int index)
        {
            return completedObjective[index];
        }

    }
}