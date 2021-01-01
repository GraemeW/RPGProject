using RPG.Quests;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.UI.Quests
{
    public class QuestTooltipUI : MonoBehaviour
    {
        [SerializeField] QuestObjectiveUI objectivePrefab = null;
        [SerializeField] Transform objectivesContainer = null;

        public void Setup(QuestStatus questStatus)
        {
            Quest quest = questStatus.GetQuest();
            for (int i = 0; i < quest.GetObjectiveCount(); i++)
            {
                // assumes indexing of statuses matches details -- this must be maintained
                string objectiveDetail = quest.GetObjectiveDetails()[i];
                bool objectiveStatus = questStatus.GetObjectiveStatusForIndex(i);

                QuestObjectiveUI objective = Instantiate(objectivePrefab, objectivesContainer);
                objective.Setup(objectiveDetail, objectiveStatus);
            }
        }
    }
}
