using RPG.Saving;
using RPG.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Inventories
{
    public class Purse : MonoBehaviour, IItemStore, ISaveable
    {
        // Tunables
        [SerializeField] float startingBalance = 400f;

        // State
        LazyValue<float> balance;

        // Event
        public event Action onChange;

        public static Purse GetPlayerPurse()
        {
            var player = GameObject.FindGameObjectWithTag("Player");
            if (player == null) { return null; }
            return player.GetComponent<Purse>();
        }

        private void Awake()
        {
            balance = new LazyValue<float>(GetInitialBalance);
        }

        private float GetInitialBalance()
        {
            return startingBalance;
        }

        public float GetBalance()
        {
            return balance.value;
        }

        public void UpdateBalance(float amount)
        {
            balance.value += amount;

            if (onChange != null)
            {
                onChange.Invoke();
            }
        }

        public int AddItems(InventoryItem item, int number)
        {
            CurrencyItem currency = item as CurrencyItem;
            if (currency == null) { return 0; }

            UpdateBalance(number * currency.GetPrice());
            return number;
        }

        public object CaptureState()
        {
            return balance.value;
        }

        public void RestoreState(object state)
        {
            balance.value = (float)state;
        }
    }
}
