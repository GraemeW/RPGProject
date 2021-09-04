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
        [SerializeField] Image icon = null;
        [SerializeField] TMP_Text nameField = null;
        [SerializeField] TMP_Text quantityField = null;
        [SerializeField] TMP_Text priceField = null;
        [SerializeField] TMP_Text quantityInTransactionField = null;

        public void Setup(ShopItem shopItem)
        {
            icon.sprite = shopItem.GetIcon();
            nameField.text = shopItem.GetName();
            quantityField.text = shopItem.GetAvailability().ToString();
            priceField.text = $"${shopItem.GetPrice():N2}";
            quantityInTransactionField.text = shopItem.GetQuantityInTransaction().ToString();
        }
    }
}
