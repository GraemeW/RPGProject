using RPG.Inventories;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Shops
{
    public class ShopItem
    {
        InventoryItem inventoryItem = null;
        int availability = 0;
        float price = 0f;
        int quantityInTransaction = 0;

        public ShopItem(InventoryItem inventoryItem, int availability, float price, int quantityInTransaction)
        {
            this.inventoryItem = inventoryItem;
            this.availability = availability;
            this.price = price;
            this.quantityInTransaction = quantityInTransaction;
        }

        public string GetName()
        {
            return inventoryItem.GetDisplayName();
        }

        public Sprite GetIcon()
        {
            return inventoryItem.GetIcon();
        }

        public int GetAvailability()
        {
            return availability;
        }

        public float GetPrice()
        {
            return price;
        }

        public int GetQuantityInTransaction()
        {
            return quantityInTransaction;
        }
    }
}