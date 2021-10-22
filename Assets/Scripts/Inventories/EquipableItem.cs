using UnityEngine;
using RPG.Core;
using System.Collections.Generic;
using System.Linq;
using System;

namespace RPG.Inventories
{
    /// <summary>
    /// An inventory item that can be equipped to the player. Weapons could be a
    /// subclass of this.
    /// </summary>
    [CreateAssetMenu(menuName = ("Inventory/Equipable Item"))]
    public class EquipableItem : InventoryItem
    {
        // CONFIG DATA
        [Tooltip("Where are we allowed to put this item.")]
        [SerializeField] EquipLocation allowedEquipLocation = EquipLocation.Weapon;
        [SerializeField] Condition equipCondition = new Condition();

        // PUBLIC
        public bool CanEquip(EquipLocation equipLocation, Equipment equipment)
        {
            if (equipLocation != allowedEquipLocation) { return false; }
            return equipCondition.Check(equipment.GetComponents<IPredicateEvaluator>());
        }

        public bool CanEquip(EquipLocation equipLocation)
        {
            return (equipLocation == allowedEquipLocation);
        }
    }
}