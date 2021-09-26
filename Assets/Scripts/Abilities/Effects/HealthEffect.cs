using RPG.Attributes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Abilities.Effects
{
    [CreateAssetMenu(fileName = "Tag Filter", menuName = "Abilities/Effects/Damage", order = 0)]
    public class HealthEffect : EffectStrategy
    {
        [SerializeField] float healthChange = 0f;

        public override void StartEffect(GameObject user, IEnumerable<GameObject> targets, Action finished)
        {
            foreach (GameObject target in targets)
            {
                if (!target.TryGetComponent(out Health health)) { continue; }

                if (health.IsDead()) { continue; }

                if (healthChange < 0)
                {
                    health.TakeDamage(user, Mathf.Abs(healthChange));
                }
                else
                {
                    health.Heal(healthChange);
                }
            }
            finished.Invoke();
        }
    }
}
