using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Attributes;
using UnityEngine.Events;

namespace RPG.Combat
{
    public class Projectile : MonoBehaviour
    {
        // Tunables
        [SerializeField] float speed = 15.0f;
        [SerializeField] float maxLifeTime = 8.0f;
        [SerializeField] bool isHoming = false;
        [SerializeField] GameObject hitEffectPrefab = null;

        // State
        Health target = null;
        Vector3 targetPoint = default;
        GameObject instigator = null;
        float damage = 0f;
        float timeAlive = 0f;

        // Events
        public UnityEvent spawn;
        public UnityEvent hit;

        private void Start()
        {
            transform.LookAt(GetAimLocation());
            spawn.Invoke();
        }

        private void Update()
        {
            MoveToTarget();
            CheckForTimeout();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject == instigator) { return; } // No self-collisions

            if (!other.TryGetComponent(out Health colliderHealth)) { return; }
            if (target != null && colliderHealth != target) { return; } // special handling for target set (e.g. homing & otherwise)
            if (target != null && target.IsDead()) { return; }

            hit.Invoke();
            TriggerVFX();
            colliderHealth.TakeDamage(instigator, damage);
            speed = 0f;
            Destroy(gameObject);
        }

        private void TriggerVFX()
        {
            if (hitEffectPrefab != null)
            {
                Instantiate(hitEffectPrefab, transform.position, Quaternion.identity);
            }
        }

        private void MoveToTarget()
        {
            if (target != null && isHoming)
            {
                if (target.IsDead()) { Destroy(gameObject); }
                transform.LookAt(GetAimLocation());
            }
            float moveDistance = Time.deltaTime * speed;
            transform.Translate(Vector3.forward * moveDistance);
        }

        private void CheckForTimeout()
        {
            timeAlive += Time.deltaTime;
            if (timeAlive > maxLifeTime)
            {
                Destroy(gameObject);
            }
        }

        public void SetTarget(GameObject instigator, Health target, float damage)
        {
            SetTarget(instigator, damage, target);
        }

        public void SetTarget(GameObject instigator, Vector3 targetPoint, float damage)
        {
            SetTarget(instigator, damage, null, targetPoint);
        }

        private void SetTarget(GameObject instigator, float damage, Health target = null, Vector3 targetPoint = default)
        {
            this.instigator = instigator;
            this.targetPoint = targetPoint;
            this.target = target;
            this.damage = damage;
        }

        private Vector3 GetAimLocation()
        {
            if (target == null)
            {
                return targetPoint;
            }

            CapsuleCollider targetCapsuleCollider = target.GetComponent<CapsuleCollider>();
            if (targetCapsuleCollider == null) { return target.transform.position; }
            return target.transform.position + Vector3.up * targetCapsuleCollider.height / 2;
        }
    }

}