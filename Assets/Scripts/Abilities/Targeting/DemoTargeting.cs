using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Abilities
{
    [CreateAssetMenu(fileName = "Demo Targeting", menuName = "Abilities/Targeting/Demo")]
    public class DemoTargeting : TargetingStrategy
    { 
        public override void StartTargeting(AbilityData abilityData, Action finished)
        {
            UnityEngine.Debug.Log("Demo Targeting Started");

            finished.Invoke();
        }
    }

}
