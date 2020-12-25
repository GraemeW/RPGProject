using RPG.Attributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Inventories
{
    public class SelfSpawnEquipment : StatsEquipment
    {
        // Tunables
        [SerializeField] EquipmentSet[] equippedItems = default;

        [System.Serializable]
        class EquipmentSet
        {
            public EquipLocation equipLocation = EquipLocation.Weapon;
            public string equippedItemID = "";
        }

        private void Start()
        {
            if (GetComponent<Health>().IsDead()) { return; }

            foreach (EquipmentSet equipmentSet in equippedItems)
            {
                EquipableItem itemToEquip = InventoryItem.GetFromID(equipmentSet.equippedItemID) as EquipableItem;
                if (itemToEquip != null)
                {
                    AddItem(equipmentSet.equipLocation, itemToEquip);
                }
            }
        }
    }
}