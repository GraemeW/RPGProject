using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Inventories;

namespace RPG.Abilities
{
    [CreateAssetMenu(menuName = ("Abilities/Ability"))]
    public class Ability : ActionItem
    {
        [SerializeField] TargetingStrategy targetingStrategy = null;
        [SerializeField] FilterStrategy[] filterStrategies = null;
        [SerializeField] EffectStrategy[] effectStrategies = null;

        public override void Use(GameObject user)
        {
            targetingStrategy.StartTargeting(user, 
                (IEnumerable<GameObject> targets) => { TargetAcquired(user, targets); });
        }

        private void TargetAcquired(GameObject user, IEnumerable<GameObject> targets)
        {
            if (filterStrategies != null)
            {
                foreach (FilterStrategy filterStrategy in filterStrategies)
                {
                    targets = filterStrategy.Filter(targets);
                }
            }
            if (targets == null) { return; }

            foreach (EffectStrategy effectStrategy in effectStrategies)
            {
                effectStrategy.StartEffect(user, targets, EffectFinished);
            }
        }

        private void EffectFinished()
        {

        }
    }
}
