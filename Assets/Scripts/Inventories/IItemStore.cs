using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Inventories
{
    public interface IItemStore
    {
        int AddItems(InventoryItem item, int number);
    }
}