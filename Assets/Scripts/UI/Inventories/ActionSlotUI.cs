using System.Collections;
using System.Collections.Generic;
using RPG.Abilities;
using RPG.Core.UI.Dragging;
using RPG.Inventories;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.UI.Inventories
{
    /// <summary>
    /// The UI slot for the player action bar.
    /// </summary>
    public class ActionSlotUI : MonoBehaviour, IItemHolder, IDragContainer<InventoryItem>
    {
        // CONFIG DATA
        [SerializeField] InventoryItemIcon icon = null;
        [SerializeField] Image cooldownOverlay = null;
        [SerializeField] int index = 0;

        // CACHE
        ActionStore store;
        CooldownStore cooldownStore;

        // LIFECYCLE METHODS
        private void Awake()
        {
            GameObject playerGameObject = GameObject.FindGameObjectWithTag("Player");
            store = playerGameObject.GetComponent<ActionStore>();
            store.storeUpdated += UpdateIcon;
            cooldownStore = playerGameObject.GetComponent<CooldownStore>();
        }

        private void FixedUpdate()
        {
            InventoryItem inventoryItem = GetItem();
            if (inventoryItem == null) { return; }

            cooldownOverlay.fillAmount = cooldownStore.GetFractionRemaining(inventoryItem);
        }

        // PUBLIC

        public void AddItems(InventoryItem item, int number)
        {
            store.AddAction(item, index, number);
        }

        public InventoryItem GetItem()
        {
            return store.GetAction(index);
        }

        public int GetNumber()
        {
            return store.GetNumber(index);
        }

        public int MaxAcceptable(InventoryItem item)
        {
            return store.MaxAcceptable(item, index);
        }

        public void RemoveItems(int number)
        {
            store.RemoveItems(index, number);
        }

        // PRIVATE

        void UpdateIcon()
        {
            icon.SetItem(GetItem(), GetNumber());
        }
    }
}
