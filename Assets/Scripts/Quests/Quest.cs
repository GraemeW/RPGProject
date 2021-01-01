using System.Collections;
using System.Collections.Generic;
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
        [SerializeField] string[] objectives = null;

        // State
        static Dictionary<string, Quest> questLookupCache;

        // Methods
        public string GetUniqueID()
        {
            return uniqueID;
        }

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

        public string GetDetail()
        {
            return detail;
        }

        public int GetObjectiveCount()
        {
            return objectives.Length;
        }

        public string[] GetObjectiveDetails()
        {
            return objectives;
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