using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Quests
{
    public class QuestList : MonoBehaviour
    {
        [SerializeField] List<QuestStatus> statuses;

        public IEnumerable<QuestStatus> GetQuestStatuses()
        {
            return statuses;
        }
    }

}