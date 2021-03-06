﻿using UnityEngine;
using RPG.Attributes;
using RPG.Inventories;
using RPG.Stats;
using System.Collections.Generic;

namespace RPG.Combat
{
    [CreateAssetMenu(fileName = "Weapon", menuName = "Weapons/New Weapon", order = 0)]
    public class WeaponConfig : EquipableItem, IModifierProvider
    {
        // Tunables
        [Header("Weapon Graphics")]
        [SerializeField] Weapon weaponPrefab = null;
        [SerializeField] AnimatorOverrideController animatorOverride = null;
        [Header("Weapon Properties")]
        [SerializeField] bool isRightHanded = true;
        [SerializeField] float weaponRange = 2.0f;
        [SerializeField] float timeBetweenAttacks = 1.5f;
        [SerializeField] float weaponDamage = 5.0f;
        [SerializeField] float weaponPercentageBonus = 0.0f;
        [Header("Ranged Specific")]
        [SerializeField] Projectile projectilePrefab = null;
        [SerializeField] float projectileOffset = 0.5f;

        // Constants
        const string WEAPON_NAME = "Weapon";

        public Weapon Spawn(Transform rightHand, Transform leftHand, Animator animator)
        {
            UpdateAttackAnimation(animator);

            if (weaponPrefab == null) { return null; }
            Transform hand = GetHandedness(rightHand, leftHand);
            Weapon spawnedWeapon = Instantiate(weaponPrefab, hand);
            spawnedWeapon.transform.parent = hand;
            return spawnedWeapon;
        }

        private void UpdateAttackAnimation(Animator animator)
        {
            var overrideController = animator.runtimeAnimatorController as AnimatorOverrideController; // Checking for existence of override controller
            if (animatorOverride != null) { animator.runtimeAnimatorController = animatorOverride; }
            else if (overrideController != null)
            {
                animator.runtimeAnimatorController = overrideController.runtimeAnimatorController; // Restore original animator controller from the override
            }
        }

        public void DestroyOldWeapon(Transform rightHand, Transform leftHand) // Unused, tracking weapons equipped in Fighter
        {
            Transform oldWeapon = rightHand.Find(WEAPON_NAME);
            if (oldWeapon == null)
            {
                oldWeapon = leftHand.Find(WEAPON_NAME);
                if (oldWeapon == null) { return; }
            }
            oldWeapon.name = "DESTROYING";
            Destroy(oldWeapon);
        }

        public void LaunchProjectile(GameObject instigator, Transform rightHand, Transform leftHand, Health target, float calculatedDamage)
        {
            if (projectilePrefab == null) { return; }
            Transform hand = GetHandedness(rightHand, leftHand);

            Projectile projectile = Instantiate(projectilePrefab, hand.position + Vector3.forward * projectileOffset, Quaternion.identity);
            projectile.SetTarget(instigator, target, calculatedDamage);
        }

        private Transform GetHandedness(Transform rightHand, Transform leftHand)
        {
            Transform hand = rightHand;
            if (!isRightHanded) { hand = leftHand; }

            return hand;
        }

        public float GetWeaponRange()
        {
            return weaponRange;
        }

        public float GetTimeBetweenAttacks()
        {
            return timeBetweenAttacks;
        }

        public float GetWeaponDamage()
        {
            return weaponDamage;
        }

        public float GetPercentageBonus()
        {
            return weaponPercentageBonus;
        }

        public bool HasProjectile()
        {
            return projectilePrefab != null;
        }

        public IEnumerable<float> GetAdditiveModifiers(Stat stat)
        {
            if (stat == Stat.Damage)
            {
                yield return weaponDamage;
            }
        }

        public IEnumerable<float> GetPercentageModifiers(Stat stat)
        {
            if (stat == Stat.Damage)
            {
                yield return weaponPercentageBonus;
            }
        }
    }
}
