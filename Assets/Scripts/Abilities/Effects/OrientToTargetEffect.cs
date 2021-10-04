using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Abilities.Effects
{
    [CreateAssetMenu(fileName = "Orient To Target Effect", menuName = "Abilities/Effects/OrientToTarget", order = 0)]
    public class OrientToTargetEffect : EffectStrategy
    {
        public override void StartEffect(AbilityData abilityData, Action finished)
        {
            abilityData.GetUser().transform.LookAt(abilityData.GetTargetedPoint());
            finished.Invoke();
        }
    }
}