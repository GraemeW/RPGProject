using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Inventories
{
    [CreateAssetMenu(menuName = ("Inventory/Drop Library"))]
    public class DropLibrary : ScriptableObject
    {
        // Tunables
        [SerializeField] float[] dropChancePercentage;
        [SerializeField] int[] minDrops;
        [SerializeField] int[] maxDrops;
        [SerializeField] DropConfig[] potentialDrops;

        [System.Serializable]
        class DropConfig
        {
            public InventoryItem item = null;
            public float[] relativeChance;
            public int[] minItems;
            public int[] maxItems;
            public int GetRandomNumber(int level)
            {
                if (!item.IsStackable())
                {
                    return 1;
                }
                int min = GetByLevel(minItems, level);
                int max = GetByLevel(maxItems, level);
                return Random.Range(min, max + 1);
            }
        }

        // Static
        static float[] totalChance = new float[99];

        // Implementation

        public struct Dropped
        {
            public InventoryItem item;
            public int number;
        }
        public IEnumerable<Dropped> GetRandomDrops(int level)
        {
            if (!ShouldRandomDrop(level))
            {
                yield break;
            }

            int numberOfDrops = GetRandomNumberOfDrops(level);
            for (int dropIndex = 0; dropIndex < numberOfDrops; dropIndex++)
            {
                yield return GetRandomDrop(level);
            }
        }

        private bool ShouldRandomDrop(int level)
        {
            return Random.value < (GetByLevel(dropChancePercentage, level) / 100f);
        }

        private int GetRandomNumberOfDrops(int level)
        {
            return Random.Range(GetByLevel(minDrops, level), GetByLevel(maxDrops, level) + 1);
        }

        Dropped GetRandomDrop(int level)
        {
            DropConfig dropConfig = SelectRandomItem(level);

            Dropped dropped = new Dropped();
            if (dropConfig != null)
            {
                dropped.item = dropConfig.item;
                dropped.number = dropConfig.GetRandomNumber(level);
            }
            return dropped;
        }

        DropConfig SelectRandomItem(int level)
        {
            GetTotalChance(level);
            float randomRoll = Random.Range(0f, totalChance[level - 1]);

            float chanceTotal = 0f;
            foreach (DropConfig drop in potentialDrops)
            {
                chanceTotal += GetByLevel(drop.relativeChance, level);
                if (chanceTotal > randomRoll)
                {
                    return drop;
                }
            }
            return null;
        }

        private void GetTotalChance(int level)
        {
            if (totalChance.Length < level) { totalChance = new float[level]; }
            
            if (Mathf.Approximately(totalChance[level - 1], 0f))
            {
                foreach (DropConfig drop in potentialDrops)
                {
                    totalChance[level - 1] += GetByLevel(drop.relativeChance, level);
                }
            }
        }

        static T GetByLevel<T>(T[] values, int level)
        {
            if (values.Length == 0)
            {
                return default;
            }
            if (level > values.Length)
            {
                return values[values.Length - 1];
            }
            if (level <= 0)
            {
                return default;
            }
            return values[level - 1];
        }
    }
}