using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Abilities.Effects
{
    [CreateAssetMenu(fileName = "Trigger Animation Effect", menuName = "Abilities/Effects/TriggerAnimation", order = 0)]
    public class TriggerAnimationEffect : EffectStrategy
    {
        [SerializeField] string animationTrigger = "ability1";

        public override void StartEffect(AbilityData abilityData, Action<EffectStrategy> finished)
        {
            if (abilityData.GetUser().TryGetComponent(out Animator animator))
            {
                animator.SetTrigger(animationTrigger);
                finished.Invoke(this);
            }
        }
    }
}
