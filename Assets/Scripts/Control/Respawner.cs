using RPG.Attributes;
using RPG.SceneManagement;
using RPG.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace RPG.Control
{
    public class Respawner : MonoBehaviour
    {
        // Tunables
        [SerializeField] Transform respawnLocation = null;
        [SerializeField] float secondsBeforeRespawn = 5.0f;

        // Cached References
        Health health = null;
        LazyValue<SavingWrapper> savingWrapper;

        private void Awake()
        {
            health = GetComponent<Health>();
            health.onDie.AddListener(Respawn);

            savingWrapper = new LazyValue<SavingWrapper>(() => FindObjectOfType<SavingWrapper>());
        }

        private void Start()
        {
            savingWrapper.ForceInit();
            if (health.IsDead())
            {
                Respawn();
            }
        }

        private void Respawn()
        {
            if (savingWrapper == null) { return; }

            // Call coroutine on savingWrapper, which persists on scene transitions
            savingWrapper.value.GetComponent<MonoBehaviour>().StartCoroutine(RespawnToScene());
        }

        private IEnumerator RespawnToScene()
        {
            savingWrapper.value.Save();
            yield return new WaitForSeconds(secondsBeforeRespawn);
            yield return savingWrapper.value.ReloadFirstScene(GetActionsOnRespawn());
        }

        private IEnumerable<Action> GetActionsOnRespawn()
        {
            List<Action> actions = new List<Action>();
            // Potential loss of player reference, re-find & use for subsequent calls
            actions.Add(() => GameObject.FindGameObjectWithTag("Player").GetComponent<NavMeshAgent>().Warp(respawnLocation.position));
            actions.Add(() => GameObject.FindGameObjectWithTag("Player").GetComponent<Health>().Revive());
            actions.Add(() => AIController.ResetDisposition()); // in spawning scene, prevent shenanigans

            return actions;
        }
    }
}
