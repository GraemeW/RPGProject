using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Movement;
using RPG.Core;
using RPG.Saving;
using RPG.Resources;
using RPG.Stats;
using RPG.Utils;

namespace RPG.Combat
{
    public class Fighter : MonoBehaviour, IAction, ISaveable, IModifierProvider
    {
        // Tunables
        [Header("Weapon")]
        [SerializeField] Transform rightHand = null;
        [SerializeField] Transform leftHand = null;
        [SerializeField] string defaultWeaponName = "Unarmed";
        [SerializeField] Weapon unarmed = null;
        float dropWeaponOffset = 3.0f;

        [Header("Chase")]
        [SerializeField] float chaseSpeedFraction = 0.95f;

        // Cached References
        Mover mover = null;
        ActionScheduler actionScheduler = null;
        Animator animator = null;
        Health health = null;
        BaseStats baseStats = null;

        // State
        private Health target = null;
        float timeSinceLastAttack = Mathf.Infinity;
        LazyValue<Weapon> currentWeapon;
        GameObject spawnedWeaponInstance = null;

        private void Awake()
        {
            mover = GetComponent<Mover>();
            health = GetComponent<Health>();
            actionScheduler = GetComponent<ActionScheduler>();
            animator = GetComponent<Animator>();
            baseStats = GetComponent<BaseStats>();
            currentWeapon = new LazyValue<Weapon>(GetInitialWeapon);
        }

        private Weapon GetInitialWeapon()
        {
            return UnityEngine.Resources.Load<Weapon>(defaultWeaponName);
        }

        private void Start()
        {
            currentWeapon.ForceInit();
            if (!health.IsDead()) { spawnedWeaponInstance = currentWeapon.value.Spawn(rightHand, leftHand, animator); }
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
            if (health.IsDead()) { return; }
            DropWeapon();
            currentWeapon.value = weapon;
            spawnedWeaponInstance = weapon.Spawn(rightHand, leftHand, animator);
        }

        public void DropWeapon()
        {
            if (spawnedWeaponInstance == null) { return; }
            Destroy(spawnedWeaponInstance);

            WeaponPickup weaponPickupPrefab = currentWeapon.value.GetWeaponPickup();
            if (weaponPickupPrefab == null ) { return; }

            Vector3 pickupPosition = transform.position + transform.forward * dropWeaponOffset + transform.up * dropWeaponOffset * 0.5f;
            WeaponPickup weaponPickup = Instantiate(weaponPickupPrefab, pickupPosition, Quaternion.identity);
            weaponPickup.SetRespawning(false);

            currentWeapon.value = unarmed;
            currentWeapon.value.Spawn(rightHand, leftHand, animator);
        }

        public void DestroyWeapon()
        {
            if (spawnedWeaponInstance == null) { return; }
            Destroy(spawnedWeaponInstance);
            currentWeapon.value = unarmed;
        }    

        private void AttackBehavior()
        {
            transform.LookAt(target.transform.position);
            if (timeSinceLastAttack > currentWeapon.value.GetTimeBetweenAttacks())
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
            return Vector3.Distance(transform.position, target.transform.position) < currentWeapon.value.GetWeaponRange();
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

        public IEnumerable<float> GetAdditiveModifiers(Stat stat)
        {
            if (stat == Stat.Damage)
            {
                yield return currentWeapon.value.GetWeaponDamage();
            }
        }

        public IEnumerable<float> GetPercentageModifiers(Stat stat)
        {
            if (stat == Stat.Damage)
            {
                yield return currentWeapon.value.GetPercentageBonus();
            }
        }

        // Animation Event
        public void Hit()
        {
            if (target == null) { return; }
            target.TakeDamage(gameObject, GetCalculatedDamage());
        }

        public void Shoot()
        {
            if (target == null) { return; }
            if (!currentWeapon.value.HasProjectile()) { return; }
            currentWeapon.value.LaunchProjectile(gameObject, rightHand, leftHand, target, GetCalculatedDamage());
        }

        private float GetCalculatedDamage()
        {
            return baseStats.GetStat(Stat.Damage);
        }

        public object CaptureState()
        {
            string currentWeaponName = unarmed.name;
            if (currentWeapon != null) { currentWeaponName = currentWeapon.value.name; }
            return currentWeaponName;
        }

        public void RestoreState(object state)
        {
            currentWeapon.value = UnityEngine.Resources.Load<Weapon>((string)state);
        }
    }
}
