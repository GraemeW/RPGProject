using RPG.Core;
using RPG.Inventories;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RPG.Quests
{
    [CreateAssetMenu(fileName = "Quest", menuName = "Quests/New Quest")]
    public class Quest : ScriptableObject, ISerializationCallbackReceiver
    {
        // Tunables
        [Tooltip("Auto-generated UUID for saving/loading. Clear this field if you want to generate a new one.")]
        [SerializeField] string uniqueID = null;
        [SerializeField] string detail = "";
        [SerializeField] List<Objective> objectives = new List<Objective>();
        [SerializeField] List<Reward> rewards = new List<Reward>();

        // State
        static Dictionary<string, Quest> questLookupCache;

        // Data Structures
        [System.Serializable]
        public class Reward
        {
            [Min(1)] public int number = 0;
            public InventoryItem item = null;
        }

        [System.Serializable]
        public class Objective : ISerializationCallbackReceiver
        {
            public string uniqueID = null;
            public string description = null;

            public Objective(string description)
            {
                if (string.IsNullOrWhiteSpace(uniqueID)) { uniqueID = System.Guid.NewGuid().ToString(); }
                this.description = description;
            }

            void ISerializationCallbackReceiver.OnAfterDeserialize()
            {
                if (string.IsNullOrWhiteSpace(uniqueID)) { uniqueID = System.Guid.NewGuid().ToString(); }
            }
            void ISerializationCallbackReceiver.OnBeforeSerialize()
            {
            }
        }

        // Methods
        public static Quest GetFromID(string uniqueID)
        {
            if (questLookupCache == null)
            {
                questLookupCache = new Dictionary<string, Quest>();
                var itemList = Resources.LoadAll<Quest>("");
                foreach (var item in itemList)
                {
                    if (questLookupCache.ContainsKey(item.uniqueID))
                    {
                        Debug.LogError(string.Format("Looks like there's a duplicate ID for objects: {0} and {1}", questLookupCache[item.uniqueID], item));
                        continue;
                    }

                    questLookupCache[item.uniqueID] = item;
                }
            }

            if (uniqueID == null || !questLookupCache.ContainsKey(uniqueID)) return null;
            return questLookupCache[uniqueID];
        }

        public string GetUniqueID()
        {
            return uniqueID;
        }

        public string GetDetail()
        {
            return detail;
        }

        public bool HasObjective(string objectiveID)
        {
            return objectives.Select(c => c.uniqueID).ToArray().Contains(objectiveID);
        }

        public int GetObjectiveCount()
        {
            return objectives.Count;
        }

        public IEnumerable<Objective> GetObjective()
        {
            return objectives;
        }

        public bool HasReward()
        {
            return (rewards.Count > 0);
        }

        public IEnumerable<Reward> GetRewards()
        {
            return rewards;
        }

        // Private
        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            if (string.IsNullOrWhiteSpace(uniqueID))
            {
                uniqueID = System.Guid.NewGuid().ToString();
            }
        }

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
        }
    }
}