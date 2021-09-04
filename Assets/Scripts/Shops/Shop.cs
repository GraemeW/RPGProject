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

        public IEnumerable<ShopItem> GetFilteredItems()
        {
            // Temp code for test
            yield return new ShopItem(InventoryItem.GetFromID("94d6d82e-1e7c-42d9-ae78-bfc74d0233a3"), 10, 5f, 0);
            yield return new ShopItem(InventoryItem.GetFromID("421daf64-c270-4f18-a928-986f23b8b5d2"), 20, 7f, 0);
            yield return new ShopItem(InventoryItem.GetFromID("e8a90b91-9994-4aa0-8d75-b7f3f9bcc12c"), 30, 15f, 0);
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