using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using RPG.Movement;
using RPG.Control;

namespace RPG.SceneManagement
{
    public class Portal : MonoBehaviour, IRaycastable
    {
        enum DestinationIdentifier
        {
            A, B, C, D, E
        }

        [SerializeField] int sceneIndexToLoad = 0;
        [SerializeField] Transform spawnPoint = null;
        [SerializeField] DestinationIdentifier destination = 0;

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                StartCoroutine(Transition());
            }
        }

        private IEnumerator Transition()
        {
            DontDestroyOnLoad(gameObject);

            // Fade Out
            Fader fader = FindObjectOfType<Fader>();
            fader.ToggleFade(true);
            yield return fader.Fade();
            FindObjectOfType<SavingWrapper>().Save(); // Save world state

            // Move Position
            yield return SceneManager.LoadSceneAsync(sceneIndexToLoad);
            FindObjectOfType<SavingWrapper>().Load(); // Load world state
            Portal otherPortal = GetOtherPortal();
            UpdatePlayer(otherPortal);

            FindObjectOfType<SavingWrapper>().Save();

            // Fade In
            if (fader == null) { fader = FindObjectOfType<Fader>(); }
            fader.ToggleFade(false);
            yield return fader.Fade();

            Destroy(gameObject);
        }

        private void UpdatePlayer(Portal otherPortal)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            player.GetComponent<Mover>().TeleportToPosition(otherPortal.spawnPoint.position, otherPortal.spawnPoint.rotation);
        }

        private Portal GetOtherPortal()
        {
            Portal[] portals = FindObjectsOfType<Portal>();
            Portal leadingPortal = null;
            foreach (Portal portal in portals)
            {
                if (this == portal) { continue; }
                leadingPortal = portal;
                if (portal.destination == destination) { break; }
            }
            return leadingPortal;
        }

        public CursorType GetCursorType()
        {
            return CursorType.Portal;
        }

        public bool HandleRaycast(PlayerController callingController, string interactButtonOne, string interactButtonTwo)
        {
            if (Input.GetButtonDown(interactButtonOne) || Input.GetButtonDown(interactButtonTwo))
            {
                callingController.InteractWithMovement(true);
            }
            return true;
        }
    }
}