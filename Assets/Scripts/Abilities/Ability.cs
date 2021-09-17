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

        public override void Use(GameObject user)
        {
            targetingStrategy.StartTargeting(user);
        }
    }
}
