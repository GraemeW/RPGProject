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
        Dictionary<InventoryItem, int> transaction = new Dictionary<InventoryItem, int>();
        
        // Cached References
        Shopper shopper = null;
        Purse purse = null;

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

        public void SetShopper(Shopper shopper)
        {
            this.shopper = shopper;
            if (shopper != null) { purse = shopper.GetComponent<Purse>(); }
            else { purse = null; }
        }

        public IEnumerable<ShopItem> GetFilteredItems()
        {
            return GetAllItems();
        }

        public IEnumerable<ShopItem> GetAllItems()
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
            float total = 0f;
            foreach (ShopItem shopItem in GetAllItems())
            {
                total += shopItem.GetQuantityInTransaction() * shopItem.GetPrice();
            }
            return total;
        }

        public bool CanTransact()
        {
            return true;
        }

        public void ConfirmTransaction()
        {
            if (shopper == null) { return; }
            if (!shopper.TryGetComponent<Inventory>(out Inventory inventory)) { return; }

            foreach (ShopItem shopItem in GetAllItems())
            {
                InventoryItem inventoryItem = shopItem.GetInventoryItem();
                int quantity = shopItem.GetQuantityInTransaction();

                for (int i = 0; i < quantity; i++)
                {
                    float price = shopItem.GetPrice();
                    if (purse == null || purse.GetBalance() < price) { break; }

                    if (inventory.AddToFirstEmptySlot(inventoryItem, 1))
                    {
                        purse.UpdateBalance(-price);
                        AddToTransaction(inventoryItem, -1);
                        AddToInitialStock(inventoryItem, -1);
                    }
                }
            }

            if (onChange != null)
            {
                onChange.Invoke();
            }
        }

        private void AddToInitialStock(InventoryItem inventoryItem, int quantity)
        {
            foreach (StockItemConfig stockItemConfig in stockConfiguration)
            {
                string itemID = stockItemConfig.inventoryItem.GetItemID();
                if (inventoryItem.GetItemID().Equals(itemID))
                {
                    stockItemConfig.initialStock += quantity;
                }
            }
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