using RPG.Control;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Abilities
{
    [CreateAssetMenu(fileName = "Directional Targeting", menuName = "Abilities/Targeting/Directional")]
    public class DirectionalTargeting : TargetingStrategy
    {
        // Tunables
        [SerializeField] LayerMask layerMask = new LayerMask();
        [SerializeField] float groundOffset = 1.0f;

        public override void StartTargeting(AbilityData abilityData, Action finished)
        {
            Ray ray = PlayerController.GetMouseRay();
            if (Physics.Raycast(ray, out RaycastHit raycastHit, Mathf.Infinity, layerMask))
            {
                abilityData.SetTargetedPoint(raycastHit.point + ray.direction * groundOffset / ray.direction.y);

                finished.Invoke();
            }
        }
    }
}
