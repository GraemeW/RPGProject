using RPG.Core;
using UnityEngine;

namespace RPG.Combat
{
    [CreateAssetMenu(fileName = "Weapon", menuName = "Weapons/Make New Weapon", order = 0)]
    public class Weapon : ScriptableObject
    {
        // Tunables
        [Header("Weapon Graphics")]
        [SerializeField] GameObject weaponPrefab = null;
        [SerializeField] WeaponPickup weaponPickup = null;
        [SerializeField] AnimatorOverrideController animatorOverride = null;
        [Header("Weapon Properties")]
        [SerializeField] bool isRightHanded = true;
        [SerializeField] float weaponRange = 2.0f;
        [SerializeField] float timeBetweenAttacks = 1.5f;
        [SerializeField] float weaponDamage = 5.0f;
        [Header("Ranged Specific")]
        [SerializeField] Projectile projectilePrefab = null;
        [SerializeField] float projectileOffset = 0.5f;

        // Constants
        const string WEAPON_NAME = "Weapon";

        public GameObject Spawn(Transform rightHand, Transform leftHand, Animator animator)
        {
            if (weaponPrefab == null) { return null; }

            Transform hand = GetHandedness(rightHand, leftHand);

            GameObject spawnedWeapon = Instantiate(weaponPrefab, hand);
            spawnedWeapon.transform.parent = hand;
            animator.runtimeAnimatorController = animatorOverride;
            return spawnedWeapon;
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

        public void LaunchProjectile(Transform rightHand, Transform leftHand, Health target, float damage)
        {
            if (projectilePrefab == null) { return; }
            Transform hand = GetHandedness(rightHand, leftHand);

            Projectile projectile = Instantiate(projectilePrefab, hand.position + Vector3.forward * projectileOffset, Quaternion.identity);
            projectile.SetTarget(target, damage);
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

        public WeaponPickup GetWeaponPickup()
        {
            return weaponPickup;
        }

        public bool HasProjectile()
        {
            return projectilePrefab != null;
        }
    }
}
