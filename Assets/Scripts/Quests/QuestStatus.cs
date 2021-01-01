using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace RPG.Quests
{
    [System.Serializable]
    public class QuestStatus
    {
        [SerializeField] Quest quest;
        [SerializeField] bool[] completedObjective;

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