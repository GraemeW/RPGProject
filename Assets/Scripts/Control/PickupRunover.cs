using RPG.Inventories;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Control
{
    [RequireComponent(typeof(Pickup))]
    public class PickupRunover : MonoBehaviour
    {
        // Tunables
        bool runoverEnabled = true;

        // Cached References
        Pickup pickup = null;

        private void Awake()
        {
            pickup = GetComponent<Pickup>();
        }

        private void OnCollisionEnter(Collision other)
        {
            if (other.gameObject.CompareTag("Player") && runoverEnabled)
            {
                pickup.PickupItem();
            }
        }

        public void SetRunover(bool runoverEnabled)
        {
            this.runoverEnabled = runoverEnabled;
        }
    }
}