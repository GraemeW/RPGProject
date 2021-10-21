using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Movement;
using RPG.Core;
using RPG.Saving;
using RPG.Attributes;
using RPG.Stats;
using RPG.Utils;
using RPG.Inventories;
using System;

namespace RPG.Combat
{
    public class Fighter : MonoBehaviour, IAction
    {
        // Tunables
        [Header("Weapon")]
        [SerializeField] Transform rightHand = null;
        [SerializeField] Transform leftHand = null;
        [SerializeField] string defaultWeaponName = "Unarmed";
        [SerializeField] WeaponConfig unarmed = null;

        [Header("Behavior")]
        [SerializeField] float chaseSpeedFraction = 0.95f;
        [SerializeField] float autoAttackRange = 3.5f;
        [SerializeField] LayerMask autoAttackLayerMask = new LayerMask();

        // Cached References
        Mover mover = null;
        ActionScheduler actionScheduler = null;
        Animator animator = null;
        Health health = null;
        BaseStats baseStats = null;
        Equipment equipment = null;

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
            equipment = GetComponent<Equipment>();
            currentWeaponConfig = new LazyValue<WeaponConfig>(GetInitialWeapon);

            if (equipment != null)
            {
                equipment.equipmentUpdated += UpdateWeapon;
            }
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
            if (target.IsDead())
            {
                target = FindClosestTargetInRange();
                if (target == null)
                {
                    Cancel();
                    return;
                }
            }

            bool inRange = GetIsInRange(target.transform);
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

        private Health FindClosestTargetInRange()
        {
            Health newTarget = null;
            float newTargetDistance = Mathf.Infinity;

            foreach (Health candidate in FindAllTargetsInRange())
            {
                float candidateTargetDistance = Vector3.SqrMagnitude(transform.position - candidate.transform.position);
                if (candidateTargetDistance < newTargetDistance)
                {
                    newTarget = candidate;
                    newTargetDistance = candidateTargetDistance;
                }
            }
            return newTarget;
        }

        private IEnumerable<Health> FindAllTargetsInRange()
        {
            RaycastHit[] hitsInfo = Physics.SphereCastAll(transform.position, autoAttackRange, Vector3.forward, 0f, autoAttackLayerMask);
            foreach (RaycastHit hit in hitsInfo)
            {
                if (hit.transform.TryGetComponent(out Health health) && health.gameObject != gameObject && !health.IsDead())
                {
                    yield return health;
                }
            }
        }

        private void AttachCurrentWeapon()
        {
            currentWeapon = currentWeaponConfig.value.Spawn(rightHand, leftHand, animator);
        }

        public void EquipWeapon(WeaponConfig weaponConfig)
        {
            if (health.IsDead()) { return; }
            currentWeaponConfig.value = weaponConfig;
            currentWeapon = weaponConfig.Spawn(rightHand, leftHand, animator);
        }

        private void UpdateWeapon()
        {
            WeaponConfig weaponInEquipment = equipment.GetItemInSlot(EquipLocation.Weapon) as WeaponConfig;
            if (weaponInEquipment == null) { weaponInEquipment = unarmed; }

            if (weaponInEquipment != currentWeapon)
            {
                if (currentWeapon != null) { Destroy(currentWeapon.gameObject); }
                EquipWeapon(weaponInEquipment);
            }
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

        private bool GetIsInRange(Transform targetTransform)
        {
            return Vector3.Distance(transform.position, targetTransform.transform.position) < currentWeaponConfig.value.GetWeaponRange();
        }

        public bool CanAttack(GameObject combatTarget)
        {
            if (combatTarget == null) { return false; }
            if (!mover.CanMoveTo(combatTarget.transform.position) && GetIsInRange(combatTarget.transform)) 
            { 
                return false; 
            }
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

        public Transform GetHandTransform(bool isRightHand)
        {
            if (isRightHand) { return rightHand; }
            else { return leftHand; }
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
            target.TakeDamage(gameObject, GetCalculatedDamage(target));
            if (currentWeapon != null) { currentWeapon.OnHit(); }
        }

        public void Shoot()
        {
            if (target == null) { return; }
            if (!currentWeaponConfig.value.HasProjectile()) { return; }
            currentWeaponConfig.value.LaunchProjectile(gameObject, rightHand, leftHand, target, GetCalculatedDamage(target));
        }

        private float GetCalculatedDamage(Health target)
        {
            float damage = baseStats.GetStat(Stat.Damage);
            float defence = 0f;
            if (target.TryGetComponent(out BaseStats targetBaseStats))
            {
                defence = targetBaseStats.GetStat(Stat.Defence);
            }
            damage /= (1 + defence / damage);

            return damage;
        }
    }
}
