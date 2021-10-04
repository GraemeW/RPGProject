using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Abilities.Effects
{
    [CreateAssetMenu(fileName = "Trigger Resources Cooldowns Effect", menuName = "Abilities/Effects/TriggerResourcesCooldowns", order = 0)]
    public class TriggerResourcesCooldownsEffect : EffectStrategy
    {
        public override void StartEffect(AbilityData abilityData, Action<EffectStrategy> finished)
        {
            finished.Invoke(this);
        }
    }
}