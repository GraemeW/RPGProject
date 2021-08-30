using RPG.Control;
using RPG.Inventories;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Shops
{
    public class Shop : MonoBehaviour, IRaycastable
    {
        // SerializeField
        [SerializeField] string shopName = "";

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

        public CursorType GetCursorType()
        {
            return CursorType.Shop;
        }

        public bool HandleRaycast(PlayerController callingController, string interactButtonOne = "Fire1", string interactButtonTwo = "Fire2")
        {
            if (Input.GetButtonDown(interactButtonOne))
            {
                Shopper shopper = callingController.GetComponent<Shopper>();
                shopper.SetActiveShop(this);
            }
            else if (Input.GetButtonDown(interactButtonTwo))
            {
                callingController.InteractWithMovement(true);
            }
            return true;

        }

        public string GetShopName()
        {
            return shopName;
        }
    }
}