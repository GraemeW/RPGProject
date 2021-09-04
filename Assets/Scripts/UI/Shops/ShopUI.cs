using RPG.Shops;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace RPG.UI.Shops
{
    public class ShopUI : MonoBehaviour
    {
        // Tunables
        [Header("Hookups")]
        [SerializeField] TMP_Text shopNameField = null;
        [SerializeField] Transform listRoot = null;

        [Header("Prefabs")]
        [SerializeField] RowUI rowPrefab = null;

        // State
        Shop currentShop = null;

        // Cached References
        Shopper shopper = null;

        private void Start()
        {
            SetupShopper(true);
            ShopChanged();

        }

        private void OnDestroy()
        {
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
            currentShop = shopper.GetActiveShop();
            gameObject.SetActive(currentShop != null);

            if (currentShop == null) { return; }

            shopNameField.text = currentShop.GetShopName();

            RefreshUI();
        }

        private void RefreshUI()
        {
            foreach (Transform child in listRoot)
            {
                Destroy(child.gameObject);
            }
            foreach (ShopItem shopItem in currentShop.GetFilteredItems())
            {
                RowUI rowUI = Instantiate(rowPrefab, listRoot);
                rowUI.Setup(shopItem);
            }
        }

        public void ExitShop()
        {
            shopper.SetActiveShop(null);
        }
    }
}
