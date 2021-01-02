using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using RPG.Quests;
using static RPG.Quests.Quest;

namespace RPG.UI.Quests
{
    public class QuestTooltipUI : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI detail = null;
        [SerializeField] QuestObjectiveUI objectivePrefab = null;
        [SerializeField] Transform objectivesContainer = null;
        [SerializeField] TextMeshProUGUI rewards = null;

        public void Setup(QuestStatus questStatus)
        {
            Quest quest = questStatus.GetQuest();
            detail.text = quest.GetDetail();
            SetObjectives(questStatus, quest);
            rewards.text = GetRewardText(quest);
        }

        private void SetObjectives(QuestStatus questStatus, Quest quest)
        {
            foreach (Objective objective in quest.GetObjective())
            {
                string objectiveDetail = objective.description;
                bool objectiveStatus = questStatus.GetStatusForObjectiveID(objective.uniqueID);

                QuestObjectiveUI objectiveUI = Instantiate(objectivePrefab, objectivesContainer);
                objectiveUI.Setup(objectiveDetail, objectiveStatus);
            }
        }

        private string GetRewardText(Quest quest)
        {
            string rewardText = "No reward";
            if (quest.HasReward())
            {
                List<string> stringStubs = new List<string>();
                foreach (Reward reward in quest.GetRewards())
                {
                    if (reward.number <= 0) { continue; }
                    if (reward.number == 1) { stringStubs.Add(string.Format("a {0}", reward.item.GetDisplayName())); }
                    else { stringStubs.Add(string.Format("{0}x{1}", reward.number, reward.item.GetDisplayName())); }
                }
                if (stringStubs.Count > 0)
                {
                    rewardText = string.Format("You'll get {0}", string.Join(", ", stringStubs));
                }
                
            }
            return rewardText;
        }
    }
}
