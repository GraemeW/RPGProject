using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Saving;
using System;

namespace RPG.Combat
{
    public class WeaponPickup : MonoBehaviour
    {
        [SerializeField] Weapon weapon = null;
        [SerializeField] bool respawning = true;
        [SerializeField] float respawnTime = 5.0f;

        private void Start()
        {
            if (!respawning)
            {
                Destroy(gameObject, respawnTime);
            }
        }

        public void SetRespawning(bool respawning)
        {
            this.respawning = respawning;
        }

        private void OnCollisionEnter(Collision other)
        {
            if (other.transform.CompareTag("Player"))
            {
                other.transform.GetComponent<Fighter>().EquipWeapon(weapon);
                // TODO:  Add pickup FX
                StartCoroutine(HideForSeconds(respawnTime));
            }
        }

        private IEnumerator HideForSeconds(float seconds)
        {
            HidePickup(true);
            yield return new WaitForSeconds(seconds);
            HidePickup(false);
        }

        private void HidePickup(bool hide)
        {
            GetComponent<Rigidbody>().useGravity = !hide;
            GetComponent<Collider>().enabled = !hide;
            foreach (Transform child in transform)
            {
                child.gameObject.SetActive(!hide);
            }
        }
    }

}