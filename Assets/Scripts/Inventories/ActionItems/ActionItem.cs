using RPG.Attributes;
using RPG.Stats;
using System;
using UnityEngine;

namespace RPG.Inventories
{
    /// <summary>
    /// An inventory item that can be placed in the action bar and "Used".
    /// </summary>
    /// <remarks>
    /// This class should be used as a base. Subclasses must implement the `Use`
    /// method.
    /// </remarks>
    [CreateAssetMenu(menuName = ("Inventory/Action Item"))]
    public class ActionItem : InventoryItem
    {
        // CONFIG DATA
        [Tooltip("Does an instance of this item get consumed every time it's used.")]
        [SerializeField] bool consumable = false;
        [SerializeField] Stat stat;
        [SerializeField] float statIncrement = 0f;

        // PUBLIC

        /// <summary>
        /// Trigger the use of this item. Override to provide functionality.
        /// </summary>
        /// <param name="user">The character that is using this action.</param>
        public virtual void Use(GameObject user)
        {
            UnityEngine.Debug.Log($"{user.name} is using Action Item: {GetDisplayName()}");
            if (stat == Stat.Health)
            {
                user.GetComponent<Health>().Heal(statIncrement);
            }
        }

        public bool isConsumable()
        {
            return consumable;
        }
    }
}