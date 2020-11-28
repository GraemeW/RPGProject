using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Combat
{
    public class WeaponPickup : MonoBehaviour
    {
        [SerializeField] Weapon weapon = null;

        private void OnCollisionEnter(Collision other)
        {
            if (other.transform.CompareTag("Player"))
            {
                other.transform.GetComponent<Fighter>().EquipWeapon(weapon);
                // TODO:  Add pickup FX
                Destroy(gameObject);
            }
        }
    }

}