using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Inventories
{
    public class CooldownStore : MonoBehaviour
    {
        // State
        Dictionary<InventoryItem, float> cooldownLookup = new Dictionary<InventoryItem, float>();
        Dictionary<InventoryItem, float> maxCooldownLookup = new Dictionary<InventoryItem, float>();

        private void Update()
        {
            List<InventoryItem> keys = new List<InventoryItem>(cooldownLookup.Keys);
            foreach (InventoryItem inventoryItem in keys)
            {
                cooldownLookup[inventoryItem] -= Time.deltaTime;
                if (cooldownLookup[inventoryItem] < 0f)
                { 
                    cooldownLookup.Remove(inventoryItem);
                    maxCooldownLookup.Remove(inventoryItem);
                }
            }
        }

        public void StartCooldown(InventoryItem inventoryItem, float cooldownTime)
        {
            if (cooldownLookup.ContainsKey(inventoryItem))
            {
                cooldownLookup[inventoryItem] += cooldownTime;
            }
            else
            {
                cooldownLookup[inventoryItem] = cooldownTime;
            }

            maxCooldownLookup[inventoryItem] = cooldownLookup[inventoryItem];
        }

        public float GetCooldownTimeRemaining(InventoryItem inventoryItem)
        {
            if (!cooldownLookup.ContainsKey(inventoryItem)) { return 0; }

            return cooldownLookup[inventoryItem];
        }

        public float GetFractionRemaining(InventoryItem inventoryItem)
        {
            if (inventoryItem == null) { return 0f; }
            if (!cooldownLookup.ContainsKey(inventoryItem) || !maxCooldownLookup.ContainsKey(inventoryItem)) { return 0f; }

            return Mathf.Clamp(cooldownLookup[inventoryItem] / maxCooldownLookup[inventoryItem], 0f, 1f);
        }
    }
}
