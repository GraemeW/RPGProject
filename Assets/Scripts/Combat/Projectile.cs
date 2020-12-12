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
        [SerializeField] bool homing = false;
        [SerializeField] GameObject hitEffectPrefab = null;

        // State
        Health target = null;
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
            if (other.GetComponent<Health>() != target) { return; }
            if (other.GetComponent<Health>().IsDead()) { return; }
            hit.Invoke();
            TriggerVFX();
            target.TakeDamage(instigator, damage);
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
            if (target == null) { Destroy(gameObject); }
            if (homing) { transform.LookAt(GetAimLocation()); }
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
            this.instigator = instigator;
            this.target = target;
            this.damage = damage;
        }

        private Vector3 GetAimLocation()
        {
            CapsuleCollider targetCapsuleCollider = target.GetComponent<CapsuleCollider>();
            if (targetCapsuleCollider == null) { return target.transform.position; }
            return target.transform.position + Vector3.up * targetCapsuleCollider.height / 2;
        }
    }

}