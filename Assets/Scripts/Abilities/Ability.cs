using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Inventories;
using RPG.Attributes;

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
            GameObject user = abilityData.GetUser();
            if (user.TryGetComponent(out CooldownStore cooldownStore))
            {
                cooldownStore.StartCooldown(this, cooldown);
            }
            if (user.TryGetComponent(out Mana mana))
            {
                if (!mana.UseMana(manaCost)) { return; }
            }

            if (filterStrategies != null)
            {
                foreach (FilterStrategy filterStrategy in filterStrategies)
                {
                    abilityData.SetTargets(filterStrategy.Filter(abilityData.GetTargets()));
                }
            }
            if (abilityData.GetTargets() == null) { return; }

            foreach (EffectStrategy effectStrategy in effectStrategies)
            {
                effectStrategy.StartEffect(abilityData, EffectFinished);
            }
        }

        private void EffectFinished()
        {

        }
    }
}
