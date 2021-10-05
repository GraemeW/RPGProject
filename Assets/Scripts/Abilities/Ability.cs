using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Inventories;
using RPG.Attributes;
using RPG.Core;
using RPG.Abilities.Effects;
using System;

namespace RPG.Abilities
{
    [CreateAssetMenu(menuName = ("Abilities/Ability"))]
    public class Ability : ActionItem
    {
        // Tunables
        [Header("Scriptable Object Inputs")]
        [SerializeField] TargetingStrategy targetingStrategy = null;
        [SerializeField] FilterStrategy[] filterStrategies = null;
        [SerializeField] EffectStrategy[] effectStrategies = null;
        [Header("Other Inputs")]
        [SerializeField] float cooldown = 0f;
        [SerializeField] float manaCost = 0f;

        public override void Use(GameObject user)
        {
            if (!CheckForCooldown(user)) { return; }
            if (!CheckForMana(user)) { return; }

            AbilityData abilityData = new AbilityData(user);
            if (user.TryGetComponent(out ActionScheduler actionScheduler))
            {
                actionScheduler.StartAction(abilityData);
            }

            targetingStrategy.StartTargeting(abilityData, 
                () => { TargetAcquired(abilityData); });
        }

        private bool CheckForCooldown(GameObject user)
        {
            if (user.TryGetComponent(out CooldownStore cooldownStore))
            {
                if (cooldownStore.GetCooldownTimeRemaining(this) > 0)
                {
                    return false;
                }
            }
            return true;
        }

        private bool CheckForMana(GameObject user)
        {
            if (user.TryGetComponent(out Mana mana))
            {
                if (manaCost > mana.GetMana())
                {
                    return false;
                }
            }
            return true;
        }

        private void TargetAcquired(AbilityData abilityData)
        {
            if (abilityData.IsCancelled()) { return; }

            if (filterStrategies != null)
            {
                foreach (FilterStrategy filterStrategy in filterStrategies)
                {
                    if (filterStrategy == null) { continue; }
                    abilityData.SetTargets(filterStrategy.Filter(abilityData.GetTargets()));
                }
            }
            if (abilityData.GetTargets() == null) { return; }

            foreach (EffectStrategy effectStrategy in effectStrategies)
            {
                effectStrategy.StartEffect(abilityData, (EffectStrategy childEffectStrategy) => EffectFinished(abilityData, childEffectStrategy));
            }
        }

        private void EffectFinished(AbilityData abilityData, EffectStrategy effectStrategy)
        {
            if (effectStrategy.GetType() == typeof(TriggerResourcesCooldownsEffect))
            {
                GameObject user = abilityData.GetUser();
                if (user.TryGetComponent(out Mana mana))
                {
                    if (!mana.UseMana(manaCost)) { return; }
                }
                if (user.TryGetComponent(out CooldownStore cooldownStore))
                {
                    cooldownStore.StartCooldown(this, cooldown);
                }
            }
        }
    }
}
