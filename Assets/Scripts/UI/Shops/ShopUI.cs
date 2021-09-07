using RPG.Shops;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace RPG.UI.Shops
{
    public class ShopUI : MonoBehaviour
    {
        // Tunables
        [Header("Hookups")]
        [SerializeField] TMP_Text shopNameField = null;
        [SerializeField] Transform listRoot = null;
        [SerializeField] TMP_Text totalField = null;
        [SerializeField] Button confirmButton = null;
        [SerializeField] TMP_Text switchButtonText = null;
        [SerializeField] TMP_Text confirmButtonText = null;
        [SerializeField] FilterButtonUI[] filterButtons = null;

        [Header("Prefabs")]
        [SerializeField] RowUI rowPrefab = null;

        [Header("Settings")]
        [SerializeField] Color cannotTransactColor = Color.red;
        [SerializeField] string switchBuyText = "Switch to buying";
        [SerializeField] string switchSellText = "Switch to selling";
        [SerializeField] string modeBuyText = "Buy";
        [SerializeField] string modeSellText = "Sell";

        // State
        Shop currentShop = null;
        Color originalTotalTextColor;

        // Cached References
        Shopper shopper = null;

        private void Start()
        {
            SetupShopper(true);
            ShopChanged();
            originalTotalTextColor = totalField.color;
        }

        private void OnDestroy()
        {
            if (currentShop != null) { currentShop.onChange -= RefreshUI; }
            SetupShopper(false);
        }

        private void SetupShopper(bool enable)
        {
            if (shopper == null)
            {
                GameObject playerGameObject = GameObject.FindGameObjectWithTag("Player");
                if (playerGameObject != null)
                {
                    shopper = playerGameObject.GetComponent<Shopper>();
                }
            }
            if (shopper == null) { return; }

            if (enable)
            {
                shopper.activeShopChanged += ShopChanged;
            }
            else
            {
                shopper.activeShopChanged -= ShopChanged;
            }
        }

        private void ShopChanged()
        {
            if (currentShop != null) { currentShop.onChange -= RefreshUI; }

            currentShop = shopper.GetActiveShop();
            gameObject.SetActive(currentShop != null);

            foreach (FilterButtonUI filterButton in filterButtons)
            {
                filterButton.SetShop(currentShop);
            }
            if (currentShop == null) { return; }

            currentShop.onChange += RefreshUI;
            shopNameField.text = currentShop.GetShopName();

            RefreshUI();
        }

        private void RefreshUI()
        {
            foreach (FilterButtonUI filterButton in filterButtons)
            {
                filterButton.RefreshUI();
            }

            foreach (Transform child in listRoot)
            {
                Destroy(child.gameObject);
            }
            foreach (ShopItem shopItem in currentShop.GetFilteredItems())
            {
                RowUI rowUI = Instantiate(rowPrefab, listRoot);
                rowUI.Setup(currentShop, shopItem);
            }

            totalField.text = $"${currentShop.GetTransactionTotal():N2}";

            totalField.color = currentShop.HasSufficientFunds() ? originalTotalTextColor : cannotTransactColor;
            confirmButton.interactable = currentShop.CanTransact(); ;
            UpdateConfirmButtonText();
        }

        public void ConfirmTransaction() // Called via Unity Events
        {
            currentShop.ConfirmTransaction();
        }

        public void SwitchMode() // Called via Unity Events
        {
            currentShop.SelectMode(!currentShop.IsBuyingMode());
        }

        private void UpdateConfirmButtonText()
        {
            if (currentShop.IsBuyingMode())
            {
                switchButtonText.text = switchSellText;
                confirmButtonText.text = modeBuyText;
            }
            else
            {
                switchButtonText.text = switchBuyText;
                confirmButtonText.text = modeSellText;
            }
        }

        public void ExitShop() // Called via Unity Events
        {
            shopper.SetActiveShop(null);
        }
    }
}
