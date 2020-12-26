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
            public EquipLocation location = EquipLocation.Weapon;
            public EquipableItem item = default;
        }

        private void Start()
        {
            if (GetComponent<Health>().IsDead()) { return; }

            foreach (EquipmentSet equippedItem in equippedItems)
            {
                if (equippedItem.item == null) { continue; }
                if (equippedItem.item.GetAllowedEquipLocation() == equippedItem.location)
                {
                    AddItem(equippedItem.location, equippedItem.item);
                }
            }
        }
    }
}