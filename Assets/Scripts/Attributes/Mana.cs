using RPG.Stats;
using RPG.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Attributes
{
    public class Mana : MonoBehaviour
    {
        // Tunables
        [Tooltip("in seconds")] [SerializeField] float tickPeriod = 2f;

        // State
        float maxMana = 0f;
        float manaRegenRate = 0f;
        LazyValue<float> currentMana;
        float tickTimer = 0f;

        // Cached References
        BaseStats baseStats = null;

        private void Awake()
        {
            baseStats = GetComponent<BaseStats>();

            currentMana = new LazyValue<float>(GetMaxMana);
        }

        private void Start()
        {
            currentMana.ForceInit();
            GetMaxMana();
            GetManaRegenRate();
        }

        public float GetMaxMana()
        {
            maxMana = baseStats.GetStat(Stat.Mana);
            return maxMana;
        }

        public float GetManaRegenRate()
        {
            manaRegenRate = baseStats.GetStat(Stat.ManaRegenRate);
            return manaRegenRate;
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
                currentMana.value = Mathf.Min(currentMana.value + manaRegenRate, maxMana);
                tickTimer = 0f;
            }
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