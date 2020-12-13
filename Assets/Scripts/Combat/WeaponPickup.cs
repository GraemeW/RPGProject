using System.Collections;
using UnityEngine;
using RPG.Control;
using RPG.Attributes;
using RPG.Movement;

namespace RPG.Combat
{
    public class WeaponPickup : MonoBehaviour, IRaycastable
    {
        [SerializeField] WeaponConfig weaponConfig = null;
        [SerializeField] float healthToRestore = 0.0f;
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
            Pickup(other.gameObject);
        }

        private void Pickup(GameObject subject)
        {
            if (subject.CompareTag("Player"))
            {
                // TODO:  Add pickup FX
                HandleWeaponProperties(subject);
                HandleMiscProperties(subject);
                StartCoroutine(HideForSeconds(respawnTime));
            }
        }

        private void HandleWeaponProperties(GameObject subject)
        {
            Fighter fighter = subject.GetComponent<Fighter>();
            if (fighter == null) { return; }
            if (weaponConfig == null) { return; }
            fighter.EquipWeapon(weaponConfig);
        }

        private void HandleMiscProperties(GameObject subject)
        {
            Health health = subject.GetComponent<Health>();
            if (health == null) { return; }
            health.Heal(healthToRestore);
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

        public bool HandleRaycast(PlayerController callingController, string interactButtonOne, string interactButtonTwo)
        {
            Mover callingControllerMover = callingController.GetComponent<Mover>();
            if (!callingControllerMover.CanMoveTo(transform.position)) { return false; }

            if (Input.GetButtonDown(interactButtonOne))
            {
                Pickup(callingController.gameObject);
            }
            else if (Input.GetButtonDown(interactButtonTwo))
            {
                callingController.InteractWithMovement(true);
            }
            return true;
        }

        public CursorType GetCursorType()
        {
            return CursorType.Pickup;
        }
    }

}