using RPG.Quests;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.UI.Quests
{
    public class QuestItemUI : MonoBehaviour
    {
        // Tunables
        [SerializeField] TextMeshProUGUI detail = null;
        [SerializeField] TextMeshProUGUI progress = null;

        // State
        QuestStatus questStatus = null;

        public void SetUp(QuestStatus questStatus)
        {
            if (questStatus == null) { return; }
            this.questStatus = questStatus;

            detail.text = questStatus.GetQuest().GetDetail();
            
            progress.text = string.Format("{0}/{1}", 
                questStatus.GetCompletedObjectiveCount(), 
                questStatus.GetQuest().GetObjectiveCount());
        }

        public QuestStatus GetQuestStatus()
        {
            return questStatus;
        }
    }
}
