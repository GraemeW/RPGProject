using System.Collections;
using UnityEngine;
using RPG.Control;

namespace RPG.Combat
{
    public class WeaponPickup : MonoBehaviour, IRaycastable
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
            Fighter fighter = other.transform.GetComponent<Fighter>();
            if (fighter == null) { return; }

            Pickup(fighter);
        }

        private void Pickup(Fighter fighter)
        {
            if (fighter.transform.CompareTag("Player"))
            {
               fighter.EquipWeapon(weapon);
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

        public bool HandleRaycast(PlayerController callingController, string interactButton)
        {
            Fighter fighter = callingController.GetComponent<Fighter>();
            if (fighter == null) { return false; }

            if (Input.GetButtonDown(interactButton))
            {
                Pickup(fighter);
            }
            return true;
        }

        public CursorType GetCursorType()
        {
            return CursorType.Pickup;
        }
    }

}