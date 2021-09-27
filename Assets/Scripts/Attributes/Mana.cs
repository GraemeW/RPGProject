using RPG.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Attributes
{
    public class Mana : MonoBehaviour
    {
        // Tunables
        [SerializeField] float maxMana = 200f;
        [SerializeField] float manaPerTick = 3f;
        [Tooltip("in seconds")] [SerializeField] float tickPeriod = 2f;

        // State
        LazyValue<float> currentMana;
        float tickTimer = 0f;

        private void Awake()
        {
            currentMana = new LazyValue<float>(GetMaxMana);
        }

        private void Start()
        {
            currentMana.ForceInit();
        }

        private void Update()
        {
            RegenMana();
        }

        private void RegenMana()
        {
            if (currentMana.value >= maxMana) { return; }

            tickTimer += Time.deltaTime;
            if (tickTimer > tickPeriod)
            {
                currentMana.value = Mathf.Min(currentMana.value + manaPerTick, maxMana);
                tickTimer = 0f;
            }
        }

        public float GetMaxMana()
        {
            return maxMana;
        }

        public float GetMana()
        {
            return currentMana.value;
        }

        public float GetFraction()
        {
            float manaFraction = currentMana.value / maxMana;
            return manaFraction;
        }

        public bool UseMana(float manaToUse)
        {
            if (manaToUse > currentMana.value)
            {
                return false;
            }

            currentMana.value -= manaToUse;
            return true;
        }

    }
}