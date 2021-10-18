using RPG.Saving;
using RPG.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RPG.Stats
{
    public class TraitStore : MonoBehaviour, IModifierProvider, ISaveable
    {
        // Tunables
        [SerializeField] TraitBonus[] bonusConfig;
        
        // Data Structures
        [System.Serializable]
        class TraitBonus
        {
            public Trait trait = default;
            public Stat stat = default;
            public float additiveBonusPerPoint = 0;
            public float percentageBonusPerPoint = 0;
        }

        // Cached References
        BaseStats baseStats = null;

        // State
        Dictionary<Trait, int> assignedPoints = new Dictionary<Trait, int>();
        Dictionary<Trait, int> stagedPoints = new Dictionary<Trait, int>();

        Dictionary<Stat, Dictionary<Trait, float>> additiveBonusCache = new Dictionary<Stat, Dictionary<Trait, float>>();
        Dictionary<Stat, Dictionary<Trait, float>> percentageBonusCache = new Dictionary<Stat, Dictionary<Trait, float>>();

        // Events
        public event Action onChange;

        private void Awake()
        {
            baseStats = GetComponent<BaseStats>();
            InitializeCaches();
        }

        private void InitializeCaches()
        {
            foreach (TraitBonus traitBonus in bonusConfig)
            {
                Stat stat = traitBonus.stat;
                Trait trait = traitBonus.trait;

                if (traitBonus.additiveBonusPerPoint != 0)
                {
                    if (!additiveBonusCache.ContainsKey(stat))
                    {
                        additiveBonusCache[stat] = new Dictionary<Trait, float>();
                    }

                    if (!additiveBonusCache[stat].ContainsKey(trait))
                    {
                        additiveBonusCache[stat][trait] = traitBonus.additiveBonusPerPoint;
                    }
                    else
                    {
                        additiveBonusCache[stat][trait] += traitBonus.additiveBonusPerPoint;
                    }
                }

                if (traitBonus.percentageBonusPerPoint != 0)
                {
                    if (!percentageBonusCache.ContainsKey(stat))
                    {
                        percentageBonusCache[stat] = new Dictionary<Trait, float>();
                    }

                    if (!percentageBonusCache[stat].ContainsKey(trait))
                    {
                        percentageBonusCache[stat][trait] = traitBonus.percentageBonusPerPoint;
                    }
                    else
                    {
                        percentageBonusCache[stat][trait] += traitBonus.percentageBonusPerPoint;
                    }
                }
            }
        }

        // Static Methods
        public static TraitStore GetPlayerTraitStore()
        {
            var player = GameObject.FindGameObjectWithTag("Player");
            if (player == null) { return null; }
            return player.GetComponent<TraitStore>();
        }

        // Other Methods
        public int GetProposedPoints(Trait trait)
        {
            return GetPoints(trait) + GetStagedPoints(trait);
        }
        
        private int GetPoints(Trait trait)
        {
            return assignedPoints.ContainsKey(trait) ? assignedPoints[trait] : 0;
        }

        private int GetStagedPoints(Trait trait)
        {
            return stagedPoints.ContainsKey(trait) ? stagedPoints[trait] : 0;
        }

        public bool HasStagedPoints()
        {
            return stagedPoints.Values.Any(x => x != 0);
        }

        public void AssignPoints(Trait trait, int points)
        {
            if (!CanAssignPoints(trait, points)) { return; }
            stagedPoints[trait] = GetStagedPoints(trait) + points;

            if (onChange != null)
            {
                onChange.Invoke();
            }
        }

        public bool CanAssignPoints(Trait trait, int points)
        {
            if (GetStagedPoints(trait) + points < 0) { return false; }
            if (GetUnassignedPoints() < points) { return false; }
            return true;
        }

        public void Commit()
        {
            foreach (Trait trait in stagedPoints.Keys)
            {
                assignedPoints[trait] = GetProposedPoints(trait);
            }
            stagedPoints.Clear();

            if (onChange != null)
            {
                onChange.Invoke();
            }
        }

        public int GetUnassignedPoints()
        {
            return Mathf.RoundToInt(baseStats.GetStat(Stat.TotalTraitPoints)) - assignedPoints.Values.Sum() - stagedPoints.Values.Sum();
        }

        public object CaptureState()
        {
            return assignedPoints;
        }

        public void RestoreState(object state)
        {
            Dictionary<Trait, int> savedAssignedPoints = state as Dictionary<Trait, int>;
            if (savedAssignedPoints == null) { return; }

            assignedPoints = savedAssignedPoints;
        }

        public IEnumerable<float> GetAdditiveModifiers(Stat stat)
        {
            if (!additiveBonusCache.ContainsKey(stat)) { yield break; }

            foreach (KeyValuePair<Trait, float> traitModifier in additiveBonusCache[stat])
            {
                yield return traitModifier.Value * GetPoints(traitModifier.Key);
            }
        }

        public IEnumerable<float> GetPercentageModifiers(Stat stat)
        {
            if (!percentageBonusCache.ContainsKey(stat)) { yield break; }

            foreach (KeyValuePair<Trait, float> traitModifier in percentageBonusCache[stat])
            {
                yield return traitModifier.Value * GetPoints(traitModifier.Key);
            }
        }
    }
}
