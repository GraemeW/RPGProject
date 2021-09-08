using RPG.Control;
using RPG.Inventories;
using RPG.Saving;
using RPG.Stats;
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
        ItemCategory filter = ItemCategory.None;
        Dictionary<InventoryItem, int> transaction = new Dictionary<InventoryItem, int>();
        Dictionary<InventoryItem, int> stock = new Dictionary<InventoryItem, int>();
        
        // Cached References
        Shopper shopper = null;
        BaseStats baseStats = null;
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
            public int levelToUnlock = 0;
        }

        private void Awake()
        {
            InitializeStockEntries();
        }

        public string GetShopName()
        {
            return shopName;
        }

        public bool IsBuyingMode()
        {
            return isBuying;
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
                baseStats = shopper.GetComponent<BaseStats>();
            }
            else 
            { 
                purse = null;
                inventory = null;
                baseStats = null;
            }
        }

        public IEnumerable<ShopItem> GetFilteredItems()
        {
            if (GetFilter() == ItemCategory.None)
            {
                return GetAllItems();
            }
            else
            {
                return GetAllItems().Where(x => x.GetInventoryItem().GetCategory() == GetFilter());
            }
        }

        public IEnumerable<ShopItem> GetAllItems()
        {
            int shopperLevel = GetShopperLevel();
            List<InventoryItem> inventoryItemsOnDeck = new List<InventoryItem>();
            foreach (StockItemConfig stockItemConfig in stockConfiguration)
            {
                if (stockItemConfig.levelToUnlock > shopperLevel) { continue; }

                InventoryItem inventoryItem = stockItemConfig.inventoryItem;

                if (inventoryItemsOnDeck.Contains(inventoryItem)) { continue; }
                inventoryItemsOnDeck.Add(inventoryItem);

                GetStock(inventoryItem, out int currentStock, out int transactionStock);
                int modifiedCurrentStock = currentStock - transactionStock;
                float price = GetPrice(stockItemConfig); 

                yield return new ShopItem(inventoryItem, modifiedCurrentStock, price, transactionStock);
            }
        }

        public void SelectFilter(ItemCategory itemCategory)
        {
            filter = itemCategory;

            if (onChange != null)
            {
                onChange.Invoke();
            }
        }

        public ItemCategory GetFilter()
        {
            return filter;
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

        public void AddToTransaction(InventoryItem inventoryItem, int quantity)
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

            if (IsBuyingMode())
            {
                return hasTransaction && hasFunds && hasSpace;
            }
            else
            {
                return hasTransaction;
            }
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
                int quantity = shopItem.GetQuantityInTransaction();

                for (int i = 0; i < quantity; i++)
                {
                    if (IsBuyingMode())
                    {
                        Buy(shopItem);
                    }
                    else
                    {
                        Sell(shopItem);
                    }
                }
            }

            if (onChange != null)
            {
                onChange.Invoke();
            }
        }

        private void Buy(ShopItem shopItem)
        {
            InventoryItem inventoryItem = shopItem.GetInventoryItem();
            float price = shopItem.GetPrice();

            if (purse == null || purse.GetBalance() < price) { return; }
            if (inventory == null) { return; }

            if (inventory.AddToFirstEmptySlot(inventoryItem, 1))
            {
                AddToTransaction(inventoryItem, -1);
                UpdateStock(inventoryItem, -1);
                purse.UpdateBalance(-price);
            }
        }

        private void Sell(ShopItem shopItem)
        {
            InventoryItem inventoryItem = shopItem.GetInventoryItem();
            float price = shopItem.GetPrice();

            if (purse == null) { return; }
            if (inventory == null) { return; }

            if (inventory.RemoveItems(inventoryItem, 1) == 1)
            {
                AddToTransaction(inventoryItem, -1);
                UpdateStock(inventoryItem, 1);
                purse.UpdateBalance(price);
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
                if (inventory == null) { currentStock = 0; }
                currentStock = inventory.GetQuantity(inventoryItem);
            }

            transactionStock = 0;
            transaction.TryGetValue(inventoryItem, out transactionStock);
        }

        private void UpdateStock(InventoryItem inventoryItem, int quantity)
        {
            if (stock.ContainsKey(inventoryItem))
            {
                stock[inventoryItem] += quantity;
            }
        }

        private float GetPrice(StockItemConfig stockItemConfig)
        {
            int currentLevel = GetShopperLevel();
            InventoryItem inventoryItem = stockItemConfig.inventoryItem;
            float bestDiscountFraction = stockConfiguration.Where(x => object.ReferenceEquals(x.inventoryItem, inventoryItem)).Where(x => x.levelToUnlock <= currentLevel).Min(x => x.buyingDiscountFraction);

            float price = stockItemConfig.inventoryItem.GetPrice() * bestDiscountFraction;
            if (!IsBuyingMode()) { price *= sellingDiscount; }
            return price;
        }

        private int GetShopperLevel()
        {
            if (baseStats == null) { return 0; }

            return baseStats.GetLevel();
        }

        // Interfaces
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