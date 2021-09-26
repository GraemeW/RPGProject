using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Inventories;

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

        public override void Use(GameObject user)
        {
            if (user.TryGetComponent(out CooldownStore cooldownStore))
            {
                if (cooldownStore.GetCooldownTimeRemaining(this) > 0)
                {
                    return;
                }
            }

            AbilityData abilityData = new AbilityData(user);
            targetingStrategy.StartTargeting(abilityData, 
                () => { TargetAcquired(abilityData); });
        }

        private void TargetAcquired(AbilityData abilityData)
        {
            if (abilityData.GetUser().TryGetComponent(out CooldownStore cooldownStore))
            {
                cooldownStore.StartCooldown(this, cooldown);
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
