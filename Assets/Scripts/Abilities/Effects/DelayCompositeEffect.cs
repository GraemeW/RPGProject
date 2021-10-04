using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Abilities.Effects
{
    [CreateAssetMenu(fileName = "Delay Composite Effect", menuName = "Abilities/Effects/DelayComposite", order = 0)]
    public class DelayCompositeEffect : EffectStrategy
    {
        [SerializeField] float delay = 0f;
        [SerializeField] EffectStrategy[] effectStrategies = null;
        [SerializeField] bool abortIfCancelled = true;

        public override void StartEffect(AbilityData abilityData, Action<EffectStrategy> finished)
        {
            if (effectStrategies == null || abilityData.IsCancelled()) { return; }

            abilityData.StartCoroutine(DelayedEffect(abilityData, finished));
        }

        private IEnumerator DelayedEffect(AbilityData abilityData, Action<EffectStrategy> finished)
        {
            yield return new WaitForSeconds(delay);
            if (abortIfCancelled && abilityData.IsCancelled()) { yield break; }
            foreach (EffectStrategy effectStrategy in effectStrategies)
            {
                effectStrategy.StartEffect(abilityData, finished);
            }
        }
    }
}
