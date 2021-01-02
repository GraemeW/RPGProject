using RPG.Inventories;
using RPG.Saving;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static RPG.Quests.Quest;
using static RPG.Quests.QuestStatus;

namespace RPG.Quests
{
    public class QuestList : MonoBehaviour, ISaveable
    {
        // Tunables
        List<QuestStatus> questStatuses = new List<QuestStatus>();

        // Cached References
        Inventory playerInventory = null;
        ItemDropper itemDropper = null;

        // Events
        public event Action questListUpdated;

        // Methods
        private void Awake()
        {
            playerInventory = GetComponent<Inventory>();
            itemDropper = GetComponent<ItemDropper>();
        }

        public IEnumerable<QuestStatus> GetActiveQuests()
        {
            return questStatuses.Where(c => !c.IsComplete());
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

        public void CompleteObjective(Quest quest, string objectiveID)
        {
            QuestStatus questStatus = GetQuestStatus(quest);
            if (questStatus == null) { return; }

            questStatus.SetObjective(objectiveID, true);

            if (questStatus.IsComplete())
            {
                GiveReward(quest);
            }
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

        private void GiveReward(Quest quest)
        {
            if (playerInventory == null || itemDropper == null) { return; }
            
            foreach (Reward reward in quest.GetRewards())
            {
                bool addedSuccessfully = playerInventory.AddToFirstEmptySlot(reward.item, reward.number);
                if (!addedSuccessfully)
                {
                    itemDropper.DropItem(reward.item, reward.number);
                }
            }
        }

        public object CaptureState()
        {
            List<SerializableQuestStatus> serializableQuestStatuses = new List<SerializableQuestStatus>();
            foreach (QuestStatus questStatus in questStatuses)
            {
                serializableQuestStatuses.Add(questStatus.CaptureState());
            }
            return serializableQuestStatuses;
        }

        public void RestoreState(object state)
        {
            List<SerializableQuestStatus> serializableQuestStatuses = state as List<SerializableQuestStatus>;
            if (serializableQuestStatuses == null) { return; }
            questStatuses.Clear();

            foreach (SerializableQuestStatus serializableQuestStatus in serializableQuestStatuses)
            {
                QuestStatus questStatus = new QuestStatus(serializableQuestStatus);
                questStatuses.Add(questStatus);
            }

            if (questListUpdated != null)
            {
                questListUpdated();
            }
        }
    }
}