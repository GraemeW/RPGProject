using RPG.Control;
using RPG.SceneManagement;
using RPG.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace RPG.UI
{
    public class PauseMenuUI : MonoBehaviour
    {
        // State
        float standardTimeScale = 1.0f;

        // State
        LazyValue<SavingWrapper> savingWrapper;
        LazyValue<PlayerController> playerController;

        private void Awake()
        {
            savingWrapper = new LazyValue<SavingWrapper>(() => FindObjectOfType<SavingWrapper>());
            playerController = new LazyValue<PlayerController>(GetPlayerController);
        }

        private void Start()
        {
            savingWrapper.ForceInit();
            playerController.ForceInit();
        }

        private PlayerController GetPlayerController()
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
            return (playerObject != null) ? playerObject.GetComponent<PlayerController>() : null;
        }

        private void OnEnable()
        {
            standardTimeScale = Time.timeScale;
            Time.timeScale = 0f;

            if (playerController.value !=  null) { playerController.value.enabled = false; }
        }

        private void OnDisable()
        {
            Time.timeScale = standardTimeScale;

            if (playerController.value != null) { playerController.value.enabled = true; }
        }

        public void Save()
        {
            savingWrapper.value.Save();
            gameObject.SetActive(false);
        }

        public void SaveQuit()
        {
            savingWrapper.value.Save();
            savingWrapper.value.LoadMenu();
        }
    }
}
