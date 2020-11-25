using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using RPG.Core;
using RPG.Control;

namespace RPG.Cinematics
{
    public class CinematicControlRemover : MonoBehaviour
    {
        PlayableDirector playableDirector = null;
        GameObject player = null;
        PlayerController playerController = null;

        private void Start()
        {
            playableDirector = GetComponent<PlayableDirector>();
            playableDirector.played += DisableControl;
            playableDirector.stopped += EnableControl;

            player = GameObject.FindWithTag("Player");
            playerController = player.GetComponent<PlayerController>();
        }

        private void DisableControl(PlayableDirector playableDirector)
        {
            player.GetComponent<ActionScheduler>().CancelCurrentAction();
            playerController.isEnabled = false;
        }

        private void EnableControl(PlayableDirector playableDirector)
        {
            playerController.isEnabled = true;
        }
    }

}