using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace RPG.SceneManagement
{
    public class Fader : MonoBehaviour
    {
        // Tunables
        [SerializeField] float fadeSpeed = 2.0f;

        // State
        float currentFade = 0f;
        float fadeTarget = 0f;
        Coroutine currentActiveFadeRoutine = null;

        // Cached References
        CanvasGroup canvasGroup = null;

        private void Awake()
        {
            canvasGroup = GetComponent<CanvasGroup>();
        }

        public Coroutine Fade()
        {
            if (currentActiveFadeRoutine != null)
            {
                StopCoroutine(currentActiveFadeRoutine);
            }
            currentActiveFadeRoutine = StartCoroutine(FadeRoutine());
            return currentActiveFadeRoutine;
        }

        private IEnumerator FadeRoutine()
        {
            while (Fading()) { yield return null; }
        }

        private bool Fading()
        {
            if (Mathf.Approximately(currentFade, fadeTarget)) { return false; }
            currentFade = Mathf.MoveTowards(currentFade, fadeTarget, Time.deltaTime * fadeSpeed);
            GetComponent<CanvasGroup>().alpha = currentFade;
            return true;
        }

        public void ToggleFade(bool fadeIn)
        {
            if (canvasGroup == null) { canvasGroup = GetComponent<CanvasGroup>(); }

            float initialFade = 0; 
            float fadeTarget = 1;
            if (!fadeIn) { initialFade = 1; fadeTarget = 0; }

            currentFade = Mathf.Clamp(initialFade, 0f, 1f);
            canvasGroup.alpha = currentFade;
            this.fadeTarget = Mathf.Clamp(fadeTarget, 0f, 1f);
        }

        public void FadeOutImmediate()
        {
            if (canvasGroup == null) { canvasGroup = GetComponent<CanvasGroup>(); }
            canvasGroup.alpha = 1f;
        }
    }

}