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
        [SerializeField] StockItemConfig[] stockConfiguration = null;

        // State
        bool isBuying = true;
        ItemCategory itemCategory = ItemCategory.None;
        float total = 0f;
        Dictionary<InventoryItem, int> transaction = new Dictionary<InventoryItem, int>();

        // Events
        public event Action onChange;

        // Data Classes
        [System.Serializable]
        class StockItemConfig
        {
            public InventoryItem inventoryItem;
            public int initialStock;
            [Range(0f, 2f)] public float buyingDiscountFraction = 1.0f;
        }

        public IEnumerable<ShopItem> GetFilteredItems()
        {
            List<InventoryItem> inventoryItemsOnDeck = new List<InventoryItem>();
            foreach (StockItemConfig stockItemConfig in stockConfiguration)
            {
                InventoryItem inventoryItem = stockItemConfig.inventoryItem;
                if (inventoryItemsOnDeck.Contains(inventoryItem)) { continue; }

                inventoryItemsOnDeck.Add(inventoryItem);
                GetStock(inventoryItem, stockItemConfig.initialStock, out int currentStock, out int transactionStock);
                yield return new ShopItem(inventoryItem, currentStock, stockItemConfig.inventoryItem.GetPrice() * stockItemConfig.buyingDiscountFraction, transactionStock);
            }
        }

        private void GetStock(InventoryItem inventoryItem, int initialStock, out int currentStock, out int transactionStock)
        {
            currentStock = initialStock;
            transactionStock = 0;
            if (transaction.TryGetValue(inventoryItem, out int pendingTransaction))
            {
                currentStock -= pendingTransaction;
                transactionStock = pendingTransaction;
            }
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
            this.isBuying = isBuying;
        }

        public bool IsBuyingMode()
        {
            return isBuying;
        }

        public void AddToTransaction(InventoryItem inventoryItem, int quantity)
        {
            int currentQuantity = 0;
            bool keyExists = transaction.TryGetValue(inventoryItem, out currentQuantity);

            int stockQuantity = 0;
            foreach (StockItemConfig stockItemConfig in stockConfiguration)
            {
                if (stockItemConfig.inventoryItem.GetItemID().Equals(inventoryItem.GetItemID()))
                {
                    stockQuantity = stockItemConfig.initialStock;
                    break;
                }
            }
            int updatedQuantity = Mathf.Clamp(currentQuantity + quantity, 0, stockQuantity);

            if (keyExists) { transaction[inventoryItem] = updatedQuantity; }
            else { transaction.Add(inventoryItem, updatedQuantity); }

            if (onChange != null)
            {
                onChange.Invoke();
            }
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