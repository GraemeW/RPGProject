using RPG.Attributes;
using RPG.Combat;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace RPG.Abilities.Effects
{
    [CreateAssetMenu(fileName = "Spawn Projectile Effect", menuName = "Abilities/Effects/SpawnProjectile", order = 0)]
    public class SpawnProjectileEffect : EffectStrategy
    {
        [SerializeField] Projectile projectileToSpawn = null;
        [SerializeField] float damage = 0f;
        [SerializeField] bool isRightHand = true;
        [SerializeField] bool useTargetPoint = true;

        public override void StartEffect(AbilityData abilityData, Action<EffectStrategy> finished)
        {
            GameObject user = abilityData.GetUser();
            if (!abilityData.GetUser().TryGetComponent(out Fighter fighter)) { return; }
            Vector3 spawnPosition = fighter.GetHandTransform(isRightHand).position;

            if (useTargetPoint)
            {
                SpawnProjectile(user, spawnPosition, abilityData.GetTargetedPoint());
            }
            else
            {
                IEnumerable targets = abilityData.GetTargets();
                SpawnProjectileForTargets(user, spawnPosition, targets);
            }
            finished.Invoke(this);
        }

        private void SpawnProjectileForTargets(GameObject user, Vector3 spawnPosition, IEnumerable targets)
        {
            foreach (GameObject target in targets)
            {
                if (target == user) { continue; }

                if (target.TryGetComponent(out Health targetHealth))
                {
                    SpawnProjectile(user, spawnPosition, targetHealth);
                }
            }
        }

        private void SpawnProjectile(GameObject instigator, Vector3 spawnPosition, Health target)
        {
            Projectile projectile = Instantiate(projectileToSpawn);
            projectile.transform.position = spawnPosition;
            projectile.SetTarget(instigator, target, damage);
        }

        private void SpawnProjectile(GameObject instigator, Vector3 spawnPosition, Vector3 targetPoint)
        {
            Projectile projectile = Instantiate(projectileToSpawn);
            projectile.transform.position = spawnPosition;
            projectile.SetTarget(instigator, targetPoint, damage);
        }
    }
}