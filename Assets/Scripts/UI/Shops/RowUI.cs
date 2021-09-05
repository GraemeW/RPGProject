using RPG.Shops;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.UI.Shops
{
    public class RowUI : MonoBehaviour
    {
        // Tunables
        [SerializeField] Image icon = null;
        [SerializeField] TMP_Text nameField = null;
        [SerializeField] TMP_Text quantityField = null;
        [SerializeField] TMP_Text priceField = null;
        [SerializeField] TMP_Text quantityInTransactionField = null;

        // State
        Shop currentShop = null;
        ShopItem shopItem = null;

        public void Setup(Shop shop, ShopItem shopItem)
        {
            icon.sprite = shopItem.GetIcon();
            nameField.text = shopItem.GetName();
            quantityField.text = shopItem.GetAvailability().ToString();
            priceField.text = $"${shopItem.GetPrice():N2}";
            quantityInTransactionField.text = shopItem.GetQuantityInTransaction().ToString();

            currentShop = shop;
            this.shopItem = shopItem;
        }

        public void Add()
        {
            if (currentShop == null) { return; }

            currentShop.AddToTransaction(shopItem.GetInventoryItem(), 1);
        }

        public void Remove()
        {
            if (currentShop == null) { return; }

            currentShop.AddToTransaction(shopItem.GetInventoryItem(), -1);
        }
    }
}
