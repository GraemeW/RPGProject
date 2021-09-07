using RPG.Inventories;
using RPG.Shops;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.UI.Shops
{
    public class FilterButtonUI : MonoBehaviour
    {
        // Tunables
        [SerializeField] ItemCategory itemCategory = ItemCategory.None;

        // Cached References
        Button button = null;
        Shop shop = null;

        private void Awake()
        {
            button = GetComponent<Button>();
        }

        private void Start()
        {
            button.onClick.AddListener(SelectFilter);
        }

        public void SetShop(Shop shop)
        {
            this.shop = shop;
        }

        public void RefreshUI()
        {
            button.interactable = (shop.GetFilter() != itemCategory);
        }

        public ItemCategory GetItemCategory()
        {
            return itemCategory;
        }

        private void SelectFilter()
        {
            shop.SelectFilter(itemCategory);
        }
    }
}