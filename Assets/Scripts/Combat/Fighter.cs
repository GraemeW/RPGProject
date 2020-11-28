using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Movement;
using RPG.Core;
using RPG.Saving;

namespace RPG.Combat
{
    public class Fighter : MonoBehaviour, IAction
    {
        // Tunables
        [Header("Weapon")]
        [SerializeField] Transform rightHand = null;
        [SerializeField] Transform leftHand = null;
        [SerializeField] Weapon defaultWeapon = null;
        [SerializeField] Weapon unarmed = null;
        float dropWeaponOffset = 3.0f;

        [Header("Chase")]
        [SerializeField] float chaseSpeedFraction = 0.95f;

        // Cached References
        Mover mover = null;
        ActionScheduler actionScheduler = null;
        Animator animator = null;
        Health health = null;

        // State
        private Health target = null;
        float timeSinceLastAttack = Mathf.Infinity;
        Weapon currentWeapon = null;
        GameObject spawnedWeaponInstance = null;

        private void Start()
        {
            mover = GetComponent<Mover>();
            health = GetComponent<Health>();
            actionScheduler = GetComponent<ActionScheduler>();
            animator = GetComponent<Animator>();
            EquipWeapon(defaultWeapon);
        }

        private void Update()
        {
            timeSinceLastAttack += Time.deltaTime;
            if (target == null) { return; }
            if (target.GetComponent<Health>().IsDead())
            {
                Cancel();
                return;
            }

            bool inRange = GetIsInRange();
            if (inRange)
            {
                mover.Cancel();
                AttackBehavior();
            }
            else
            {
                mover.MoveTo(target.transform.position, chaseSpeedFraction);
            }
        }

        public void EquipWeapon(Weapon weapon)
        {
            if (currentWeapon == weapon) { return; }
            if (health.IsDead()) { return; }
            DropWeapon();
            currentWeapon = weapon;
            spawnedWeaponInstance = weapon.Spawn(rightHand, leftHand, animator);
        }

        public void DropWeapon()
        {
            if (spawnedWeaponInstance == null) { return; }
            Destroy(spawnedWeaponInstance);

            WeaponPickup weaponPickup = currentWeapon.GetWeaponPickup();
            if (weaponPickup == null ) { return; }

            Vector3 pickupPosition = transform.position + transform.forward * dropWeaponOffset + transform.up * dropWeaponOffset * 0.5f;
            Instantiate(weaponPickup, pickupPosition, Quaternion.identity);

            currentWeapon = unarmed;
            currentWeapon.Spawn(rightHand, leftHand, animator);
        }

        public void DestroyWeapon()
        {
            if (spawnedWeaponInstance == null) { return; }
            Destroy(spawnedWeaponInstance);
            currentWeapon = unarmed;
        }    

        private void AttackBehavior()
        {
            transform.LookAt(target.transform.position);
            if (timeSinceLastAttack > currentWeapon.GetTimeBetweenAttacks())
            {
                // This will trigger a Hit() or Shoot() event
                TriggerAttack();
                timeSinceLastAttack = 0f;
            }
        }

        private void TriggerAttack()
        {
            animator.ResetTrigger("stopAttack");
            animator.SetTrigger("attack");
        }

        private bool GetIsInRange()
        {
            return Vector3.Distance(transform.position, target.transform.position) < currentWeapon.GetWeaponRange();
        }

        public bool CanAttack(GameObject combatTarget)
        {
            if (combatTarget == null) { return false; }
            Health targetToTest = combatTarget.GetComponent<Health>();
            return (targetToTest != null && !targetToTest.IsDead());
        }

        public void Attack(GameObject combatTarget)
        {
            actionScheduler.StartAction(this);
            this.target = combatTarget.GetComponent<Health>();
        }

        public Health GetTarget()
        {
            return target;
        }

        public void Cancel()
        {
            StopAttack();
            mover.Cancel();
            target = null;
            timeSinceLastAttack = Mathf.Infinity;
        }

        private void StopAttack()
        {
            animator.ResetTrigger("attack");
            animator.SetTrigger("stopAttack");
        }

        // Animation Event
        public void Hit()
        {
            if (target == null) { return; }
            target.TakeDamage(currentWeapon.GetWeaponDamage());
        }

        public void Shoot()
        {
            if (target == null) { return; }
            if (!currentWeapon.HasProjectile()) { return; }
            currentWeapon.LaunchProjectile(rightHand, leftHand, target, currentWeapon.GetWeaponDamage());
        }
    }
}
