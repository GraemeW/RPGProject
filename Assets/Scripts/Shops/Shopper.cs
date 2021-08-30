using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Shops
{
    public class Shopper : MonoBehaviour
    {
        // State
        Shop activeShop = null;

        // Event
        public event Action activeShopChanged;

        public void SetActiveShop(Shop shop)
        {
            activeShop = shop;
            if (activeShopChanged != null)
            {
                activeShopChanged.Invoke();
            }
        }

        public Shop GetActiveShop()
        {
            return activeShop;
        }
    }
}
