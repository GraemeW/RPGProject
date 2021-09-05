using RPG.Saving;
using RPG.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Inventories
{
    public class Purse : MonoBehaviour, ISaveable
    {
        // Tunables
        [SerializeField] float startingBalance = 400f;

        // State
        LazyValue<float> balance;

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
            UnityEngine.Debug.Log($"Balance: {balance.value}");
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
