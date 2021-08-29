using RPG.Inventories;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Shops
{
    public class Shop : MonoBehaviour
    {
        // State
        bool isBuying = true;
        ItemCategory itemCategory = ItemCategory.None;
        float total = 0f;

        // Events
        public event Action onChange;

        // Data Structures
        public class ShopItem
        {
            InventoryItem inventoryItem = null;
            int availability = 0;
            float price = 0f;
            int quantityInTransaction = 0;
        }

        public IEnumerable<ShopItem> GetFilteredItems()
        {
            return null;
        }

        public void SelectFilter(ItemCategory itemCategory)
        {
            this.itemCategory = itemCategory;
        }

        public ItemCategory GetFilter()
        {
            return itemCategory;
        }

        public void SelectMode(bool isBuying)
        {
            isBuying = this.isBuying;
        }

        public bool IsBuyingMode()
        {
            return isBuying;
        }

        public void AddToTransaction(InventoryItem inventoryItem, int quantity)
        {
            
        }

        public float GetTransactionTotal()
        {
            return total;
        }

        public bool CanTransact()
        {
            return true;
        }

        public void ConfirmTransaction()
        {

        }
    }
}