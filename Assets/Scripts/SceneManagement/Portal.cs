using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using RPG.Movement;
using RPG.Core;

namespace RPG.SceneManagement
{
    public class Portal : MonoBehaviour
    {
        enum DestinationIdentifier
        {
            A, B, C, D, E
        }

        [SerializeField] int sceneIndexToLoad = 0;
        [SerializeField] Transform spawnPoint = null;
        [SerializeField] DestinationIdentifier destination;

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                StartCoroutine(Transition());
            }
        }

        private IEnumerator Transition()
        {
            // Fade In
            Fader fader = FindObjectOfType<Fader>();
            fader.ToggleFade(true);
            yield return fader.Fade();

            // Move Position
            DontDestroyOnLoad(gameObject);
            yield return SceneManager.LoadSceneAsync(sceneIndexToLoad);
            Portal otherPortal = GetOtherPortal();
            UpdatePlayer(otherPortal);

            // Fade Out
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
    }
}