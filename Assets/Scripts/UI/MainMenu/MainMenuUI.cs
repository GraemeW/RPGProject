using RPG.SceneManagement;
using RPG.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace RPG.UI
{
    public class MainMenuUI : MonoBehaviour
    {
        // Tunables
        [Header("Main Menu")]
        [SerializeField] GameObject menuParent = null;
        [Header("New Game")]
        [SerializeField] TMP_InputField gameNameInput = null;

        // State
        LazyValue<SavingWrapper> savingWrapper;

        private void Awake()
        {
            savingWrapper = new LazyValue<SavingWrapper>(() => FindObjectOfType<SavingWrapper>());
        }

        private void Start()
        {
            savingWrapper.ForceInit();
        }

        public void Load(string saveFile) // Called by Unity Events
        {
            if (savingWrapper.value.ContinueGame(saveFile))
            {
                menuParent.SetActive(false);
            }
        }

        public void Continue() // Called by Unity Events
        {
            if (savingWrapper.value.ContinueGame())
            {
                menuParent.SetActive(false);
            }
        }

        public void NewGame() // Called by Unity Events
        {
            string saveFile = gameNameInput.text;
            if (string.IsNullOrWhiteSpace(saveFile)) { return; }

            menuParent.SetActive(false);
            savingWrapper.value.NewGame(saveFile);
        }

        public void Quit() // Called by Unity Events
        {
            Application.Quit();
        }
    }
}
