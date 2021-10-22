using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using RPG.Core;
using RPG.Inventories;
using RPG.Saving;
using static RPG.Quests.Quest;
using static RPG.Quests.QuestStatus;

namespace RPG.Quests
{
    public class QuestList : MonoBehaviour, IPredicateEvaluator, ISaveable
    {
        // Tunables
        List<QuestStatus> questStatuses = new List<QuestStatus>();

        // Cached References
        Inventory playerInventory = null;
        ItemDropper itemDropper = null;
        Equipment playerEquipment = null;

        // Events
        public event Action questListUpdated;

        // Static
        static string[] PREDICATES_ARRAY = { "HasQuest", "CompletedQuest", "CompletedObjective" };
        static string[] ITEM_PREDICATES = { "ItemEquipped" };

        // Methods
        private void Awake()
        {
            playerInventory = GetComponent<Inventory>();
            itemDropper = GetComponent<ItemDropper>();
            playerEquipment = GetComponent<Equipment>();
        }

        private void OnEnable()
        {
            playerEquipment.equipmentUpdated += CheckForEquipmentQuestObjective;
        }

        private void OnDisable()
        {
            playerEquipment.equipmentUpdated -= CheckForEquipmentQuestObjective;
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
            if (questStatus.IsComplete()) { return; }

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

        private void CheckForEquipmentQuestObjective()
        {
            foreach (EquipLocation equipLocation in playerEquipment.GetAllPopulatedSlots())
            {
                EquipableItem inventoryItem = playerEquipment.GetItemInSlot(equipLocation);

                InventoryItem.QuestObjective[] questObjectives = inventoryItem.GetItemQuestObjectives();
                if (questObjectives == null) { return; }

                foreach (InventoryItem.QuestObjective questObjective in questObjectives)
                {
                    if (questObjective.predicate == ITEM_PREDICATES[0])
                    {
                        Quest quest = Quest.GetFromID(questObjective.questID);
                        CompleteObjective(quest, questObjective.objectiveID);
                    }
                }
            }
        }

        // Interfaces

        // Predicate Evaluator
        public bool? Evaluate(string predicate, string[] parameters)
        {
            string matchingPredicate = this.MatchToPredicates(predicate, PREDICATES_ARRAY);
            if (string.IsNullOrWhiteSpace(matchingPredicate)) { return null; }

            if (predicate == PREDICATES_ARRAY[0])
            {
                return PredicateEvaluateHasQuest(parameters);
            }
            else if (predicate == PREDICATES_ARRAY[1])
            {
                return PredicateEvaluateQuestCompleted(parameters);
            }
            else if (predicate == PREDICATES_ARRAY[2])
            {
                return PredicateEvaluateObjectiveComplete(parameters);
            }
            return null;
        }

        string IPredicateEvaluator.MatchToPredicatesTemplate()
        {
            // Not evaluated -> PredicateEvaluatorExtension
            return null;
        }

        private bool PredicateEvaluateHasQuest(string[] parameters)
        {
            // Match on ANY of the quests present in parameters
            foreach (string questID in parameters)
            {
                if (HasQuest(Quest.GetFromID(questID)))
                {
                    return true;
                }
            }
            return false;
        }

        private bool? PredicateEvaluateQuestCompleted(string[] parameters)
        {
            // Match on ANY of the quests complete in parameters
            foreach (string questID in parameters)
            {
                QuestStatus questStatus = GetQuestStatus(Quest.GetFromID(questID));
                if (questStatus == null) { return false; } // i.e. haven't even started the quest, let alone completed

                if (questStatus.IsComplete())
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            return null;
        }

        private bool PredicateEvaluateObjectiveComplete(string[] parameters)
        {
            // Parameter 1:  QuestID
            QuestStatus questStatus = GetQuestStatus(Quest.GetFromID(parameters[0]));
            if (questStatus == null) { return false; }
            // Parameter 2:  ObjectiveID
            bool objectiveStatus = questStatus.GetStatusForObjectiveID(parameters[1]);
            return objectiveStatus;
        }

        // Save System
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