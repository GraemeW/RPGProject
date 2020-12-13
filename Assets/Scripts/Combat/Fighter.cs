using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Movement;
using RPG.Core;
using RPG.Saving;
using RPG.Attributes;
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
        [SerializeField] WeaponConfig unarmed = null;
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
        LazyValue<WeaponConfig> currentWeaponConfig;
        Weapon currentWeapon = null;

        private void Awake()
        {
            mover = GetComponent<Mover>();
            health = GetComponent<Health>();
            actionScheduler = GetComponent<ActionScheduler>();
            animator = GetComponent<Animator>();
            baseStats = GetComponent<BaseStats>();
            currentWeaponConfig = new LazyValue<WeaponConfig>(GetInitialWeapon);
        }

        private WeaponConfig GetInitialWeapon()
        {
            return UnityEngine.Resources.Load<WeaponConfig>(defaultWeaponName);
        }

        private void Start()
        {
            currentWeaponConfig.ForceInit();
            if (!health.IsDead()) { AttachCurrentWeapon(); }
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

        private void AttachCurrentWeapon()
        {
            currentWeapon = currentWeaponConfig.value.Spawn(rightHand, leftHand, animator);
        }

        public void EquipWeapon(WeaponConfig weaponConfig)
        {
            if (health.IsDead()) { return; }
            DropWeapon();
            currentWeaponConfig.value = weaponConfig;
            currentWeapon = weaponConfig.Spawn(rightHand, leftHand, animator);
        }

        public void DropWeapon()
        {
            if (currentWeapon == null) { return; }
            Destroy(currentWeapon.gameObject);

            WeaponPickup weaponPickupPrefab = currentWeaponConfig.value.GetWeaponPickup();
            if (weaponPickupPrefab == null ) { return; }

            Vector3 pickupPosition = transform.position + transform.forward * dropWeaponOffset + transform.up * dropWeaponOffset * 0.5f;
            WeaponPickup weaponPickup = Instantiate(weaponPickupPrefab, pickupPosition, Quaternion.identity);
            weaponPickup.SetRespawning(false);

            currentWeaponConfig.value = unarmed;
            currentWeapon = currentWeaponConfig.value.Spawn(rightHand, leftHand, animator);
        }

        private void AttackBehavior()
        {
            transform.LookAt(target.transform.position);
            if (timeSinceLastAttack > currentWeaponConfig.value.GetTimeBetweenAttacks())
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
            return Vector3.Distance(transform.position, target.transform.position) < currentWeaponConfig.value.GetWeaponRange();
        }

        public bool CanAttack(GameObject combatTarget)
        {
            if (combatTarget == null) { return false; }
            if (!mover.CanMoveTo(combatTarget.transform.position)) { return false; }
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
                yield return currentWeaponConfig.value.GetWeaponDamage();
            }
        }

        public IEnumerable<float> GetPercentageModifiers(Stat stat)
        {
            if (stat == Stat.Damage)
            {
                yield return currentWeaponConfig.value.GetPercentageBonus();
            }
        }

        // Animation Event
        public void Hit()
        {
            if (target == null) { return; }
            target.TakeDamage(gameObject, GetCalculatedDamage());
            if (currentWeapon != null) { currentWeapon.OnHit(); }
        }

        public void Shoot()
        {
            if (target == null) { return; }
            if (!currentWeaponConfig.value.HasProjectile()) { return; }
            currentWeaponConfig.value.LaunchProjectile(gameObject, rightHand, leftHand, target, GetCalculatedDamage());
        }

        private float GetCalculatedDamage()
        {
            return baseStats.GetStat(Stat.Damage);
        }

        public object CaptureState()
        {
            string currentWeaponName = unarmed.name;
            if (currentWeaponConfig != null) { currentWeaponName = currentWeaponConfig.value.name; }
            return currentWeaponName;
        }

        public void RestoreState(object state)
        {
            currentWeaponConfig.value = UnityEngine.Resources.Load<WeaponConfig>((string)state);
        }
    }
}
