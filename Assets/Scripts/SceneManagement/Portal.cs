using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using RPG.Movement;
using RPG.Control;
using RPG.Core;
using UnityEngine.Events;

namespace RPG.SceneManagement
{
    public class Portal : MonoBehaviour, IRaycastable
    {
        // Data Classes
        enum DestinationIdentifier
        {
            A, B, C, D, E
        }

        // Tunables
        [SerializeField] int sceneIndexToLoad = 0;
        [SerializeField] Transform spawnPoint = null;
        [SerializeField] DestinationIdentifier destination = 0;

        // Events
        public UnityEvent transition;

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

            transition.Invoke();
            // Fade Out
            Fader fader = FindObjectOfType<Fader>();
            fader.ToggleFade(true);
            TogglePlayerControl(false);
            yield return fader.Fade();
            FindObjectOfType<SavingWrapper>().Save(); // Save world state

            // Move Position
            yield return SceneManager.LoadSceneAsync(sceneIndexToLoad);
            TogglePlayerControl(false);
            FindObjectOfType<SavingWrapper>().Load(); // Load world state
            Portal otherPortal = GetOtherPortal();
            UpdatePlayer(otherPortal);

            FindObjectOfType<SavingWrapper>().Save();

            // Fade In
            if (fader == null) { fader = FindObjectOfType<Fader>(); }
            fader.ToggleFade(false);
            yield return fader.Fade();
            TogglePlayerControl(true);
            Destroy(gameObject);
        }

        private void UpdatePlayer(Portal otherPortal)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            player.GetComponent<Mover>().TeleportToPosition(otherPortal.spawnPoint.position, otherPortal.spawnPoint.rotation);
        }

        private void TogglePlayerControl(bool toggle)
        {
            GameObject player = GameObject.FindWithTag("Player");
            PlayerController playerController = player.GetComponent<PlayerController>();
            playerController.isEnabled = toggle;
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