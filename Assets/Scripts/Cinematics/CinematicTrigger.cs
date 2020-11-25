using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

namespace RPG.Cinematics
{
    public class CinematicTrigger : MonoBehaviour
    {
        bool isTriggered = false;

        private void OnTriggerEnter(Collider other)
        {
            if (!isTriggered && other.gameObject.CompareTag("Player"))
            {
                isTriggered = true;
                GetComponent<PlayableDirector>().Play();
            }
        }
    }
}
