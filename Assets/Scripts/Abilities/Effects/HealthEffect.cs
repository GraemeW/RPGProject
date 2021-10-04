using RPG.Attributes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Abilities.Effects
{
    [CreateAssetMenu(fileName = "Health Effect", menuName = "Abilities/Effects/Health", order = 0)]
    public class HealthEffect : EffectStrategy
    {
        [SerializeField] float healthChange = 0f;

        public override void StartEffect(AbilityData abilityData, Action<EffectStrategy> finished)
        {
            foreach (GameObject target in abilityData.GetTargets())
            {
                if (!target.TryGetComponent(out Health health)) { continue; }

                if (health.IsDead()) { continue; }

                if (healthChange < 0)
                {
                    health.TakeDamage(abilityData.GetUser(), Mathf.Abs(healthChange));
                }
                else
                {
                    health.Heal(healthChange);
                }
            }
            finished.Invoke(this);
        }
    }
}
