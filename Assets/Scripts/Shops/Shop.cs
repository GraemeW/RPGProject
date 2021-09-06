using RPG.Control;
using RPG.Inventories;
using RPG.Saving;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RPG.Shops
{
    public class Shop : MonoBehaviour, IRaycastable, ISaveable
    {
        // SerializeField
        [SerializeField] string shopName = "";
        [SerializeField] float sellingDiscount = 0.5f;
        [SerializeField] StockItemConfig[] stockConfiguration = null;

        // State
        bool isBuying = true;
        ItemCategory itemCategory = ItemCategory.None;
        Dictionary<InventoryItem, int> transaction = new Dictionary<InventoryItem, int>();
        Dictionary<InventoryItem, int> stock = new Dictionary<InventoryItem, int>();
        
        // Cached References
        Shopper shopper = null;
        Inventory inventory = null;
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

        private void Awake()
        {
            InitializeStockEntries();
        }

        private void InitializeStockEntries()
        {
            foreach (StockItemConfig stockItemConfig in stockConfiguration)
            {
                stock[stockItemConfig.inventoryItem] = stockItemConfig.initialStock;
            }
        }

        public void SetShopper(Shopper shopper)
        {
            this.shopper = shopper;
            if (shopper != null)
            { 
                purse = shopper.GetComponent<Purse>();
                inventory = shopper.GetComponent<Inventory>();
            }
            else 
            { 
                purse = null;
                inventory = null;
            }
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
                if (inventoryItemsOnDeck.Contains(inventoryItem)) { continue; } // Safety against bad input (multiple stock entry copies)

                inventoryItemsOnDeck.Add(inventoryItem);
                GetStock(inventoryItem, out int currentStock, out int transactionStock);
                float price = GetPrice(stockItemConfig); 

                yield return new ShopItem(inventoryItem, currentStock, price, transactionStock);
            }
        }

        private void GetStock(InventoryItem inventoryItem, out int currentStock, out int transactionStock)
        {
            if (IsBuyingMode())
            {
                currentStock = stock[inventoryItem];
            }
            else
            {
                currentStock = inventory.GetTotalQuantity(inventoryItem);
            }

            transactionStock = 0;
            if (transaction.TryGetValue(inventoryItem, out int pendingTransaction))
            {
                currentStock -= pendingTransaction;
                transactionStock = pendingTransaction;
            }
        }

        private float GetPrice(StockItemConfig stockItemConfig)
        {
            float price = stockItemConfig.inventoryItem.GetPrice() * stockItemConfig.buyingDiscountFraction;
            if (!IsBuyingMode()) { price *= sellingDiscount; }
            return price;
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
            transaction.Clear();

            if (onChange != null)
            {
                onChange.Invoke();
            }
        }

        public bool IsBuyingMode()
        {
            return isBuying;
        }

        public void UpdateTransaction(InventoryItem inventoryItem, int quantity)
        {
            GetStock(inventoryItem, out int currentStock, out int transactionStock);
            int updatedQuantity = Mathf.Clamp(transactionStock + quantity, 0, currentStock);

            transaction[inventoryItem] = updatedQuantity;

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
            bool hasTransaction = HasAvailableTransaction();
            bool hasFunds = HasSufficientFunds();
            bool hasSpace = HasSufficientInventorySpace();

            return hasTransaction && hasFunds && hasSpace;
        }

        public bool HasAvailableTransaction()
        {
            return transaction.Any(x => x.Value > 0);
        }

        public bool HasSufficientInventorySpace()
        {
            if (inventory == null) { return false; }

            return inventory.HasSpaceFor(transaction.SelectMany(x => Enumerable.Repeat(x.Key, x.Value)));
        }

        public bool HasSufficientFunds()
        {
            if (purse == null) { return false; }

            return GetTransactionTotal() <= purse.GetBalance();
        }

        public void ConfirmTransaction()
        {
            if (shopper == null) { return; }

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
                        UpdateTransaction(inventoryItem, -1);
                        UpdateStock(inventoryItem, -1);
                        purse.UpdateBalance(-price);
                    }
                }
            }

            if (onChange != null)
            {
                onChange.Invoke();
            }
        }

        private void UpdateStock(InventoryItem inventoryItem, int quantity)
        {
            if (stock.ContainsKey(inventoryItem))
            {
                stock[inventoryItem] += quantity;
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

        [System.Serializable]
        private struct SerializableStockEntry
        {
            public string inventoryItemID;
            public int quantity;
        }

        public object CaptureState()
        {
            SerializableStockEntry[] serializableStockEntries = new SerializableStockEntry[stock.Count];
            int stockIndex = 0;
            foreach (KeyValuePair<InventoryItem, int> stockEntry in stock)
            {
                string inventoryItemID = stockEntry.Key.GetItemID();
                int quantity = stockEntry.Value;

                SerializableStockEntry serializableStockEntry = new SerializableStockEntry
                {
                    inventoryItemID = inventoryItemID,
                    quantity = quantity
                };
                serializableStockEntries[stockIndex] = serializableStockEntry;
                stockIndex++;
            }

            return serializableStockEntries;
        }

        public void RestoreState(object state)
        {
            SerializableStockEntry[] serializableStockEntries = state as SerializableStockEntry[];
            if (serializableStockEntries == null) { return; }

            foreach (SerializableStockEntry serializableStockEntry in serializableStockEntries)
            {
                InventoryItem inventoryItem = InventoryItem.GetFromID(serializableStockEntry.inventoryItemID);
                if (inventoryItem == null) { return; }

                stock[inventoryItem] = serializableStockEntry.quantity;
            }
        }
    }
}