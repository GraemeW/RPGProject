using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Abilities
{
    [CreateAssetMenu(fileName = "Self Targeting", menuName = "Abilities/Targeting/Self")]
    public class SelfTargeting : TargetingStrategy
    {
        [SerializeField] float yOffsetForSpawning = 1.0f;

        public override void StartTargeting(AbilityData abilityData, Action finished)
        {
            GameObject user = abilityData.GetUser();

            abilityData.SetTargets(new[] { user });
            abilityData.SetTargetedPoint(user.transform.position + Vector3.up * yOffsetForSpawning);

            finished.Invoke();
        }
    }
}
