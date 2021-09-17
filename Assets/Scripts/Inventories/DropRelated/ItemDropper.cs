using System.Collections.Generic;
using UnityEngine;
using RPG.Saving;
using UnityEngine.SceneManagement;

namespace RPG.Inventories
{
    /// <summary>
    /// To be placed on anything that wishes to drop pickups into the world.
    /// Tracks the drops for saving and restoring.
    /// </summary>
    public class ItemDropper : MonoBehaviour, ISaveable
    {
        // Tunables
        [SerializeField] int listCleanupSize = 50;
        float dropOffset = 2.5f;

        // STATE
        private List<Pickup> droppedItems = new List<Pickup>();
        private List<DropRecord> otherSceneDroppedItems = new List<DropRecord>();

        // PUBLIC

        /// <summary>
        /// Create a pickup at the current position.
        /// </summary>
        /// <param name="item">The item type for the pickup.</param>
        /// <param name="number">
        /// The number of items contained in the pickup. Only used if the item
        /// is stackable.
        /// </param>
        public void DropItem(InventoryItem item, int number)
        {
            SpawnPickup(item, GetDropLocation(), number);
        }

        /// <summary>
        /// Create a pickup at the current position.
        /// </summary>
        /// <param name="item">The item type for the pickup.</param>
        public void DropItem(InventoryItem item)
        {
            SpawnPickup(item, GetDropLocation(), 1);
        }

        // PROTECTED

        /// <summary>
        /// Override to set a custom method for locating a drop.
        /// </summary>
        /// <returns>The location the drop should be spawned.</returns>
        protected virtual Vector3 GetDropLocation()
        {
            Vector3 dropLocation = transform.position + transform.forward * dropOffset + transform.up * dropOffset * 0.5f;
            return dropLocation;
        }

        // PRIVATE

        public void SpawnPickup(InventoryItem item, Vector3 spawnLocation, int number)
        {
            if (droppedItems.Count > listCleanupSize) { RemoveDestroyedDrops(); }
            Pickup pickup = item.SpawnPickup(spawnLocation, number);
            droppedItems.Add(pickup);
        }

        [System.Serializable]
        private struct DropRecord
        {
            public string itemID;
            public SerializableVector3 position;
            public int number;
            public int sceneBuildIndex;
        }

        object ISaveable.CaptureState()
        {
            RemoveDestroyedDrops();

            List<DropRecord> dropRecords = new List<DropRecord>();
            int buildIndex = SceneManager.GetActiveScene().buildIndex;

            foreach (Pickup pickup in droppedItems)
            {
                DropRecord dropRecord = new DropRecord();
                dropRecord.itemID = pickup.GetItem().GetItemID();
                dropRecord.position = new SerializableVector3(pickup.transform.position);
                dropRecord.number = pickup.GetNumber();
                dropRecord.sceneBuildIndex = buildIndex;
                dropRecords.Add(dropRecord);
            }
            dropRecords.AddRange(otherSceneDroppedItems);
            return dropRecords;
        }

        void ISaveable.RestoreState(object state)
        {
            List<DropRecord> dropRecords = (List<DropRecord>)state;
            int buildIndex = SceneManager.GetActiveScene().buildIndex;

            otherSceneDroppedItems.Clear();
            foreach (DropRecord dropRecord in dropRecords)
            {
                if (dropRecord.sceneBuildIndex == buildIndex)
                {
                    InventoryItem pickupItem = InventoryItem.GetFromID(dropRecord.itemID);
                    Vector3 position = dropRecord.position.ToVector();
                    int number = dropRecord.number;
                    SpawnPickup(pickupItem, position, number);
                }
                else
                {
                    otherSceneDroppedItems.Add(dropRecord);
                }
            }
        }

        /// <summary>
        /// Remove any drops in the world that have subsequently been picked up.
        /// </summary>
        private void RemoveDestroyedDrops()
        {
            var newList = new List<Pickup>();
            foreach (var item in droppedItems)
            {
                if (item != null)
                {
                    newList.Add(item);
                }
            }
            droppedItems = newList;
        }
    }
}