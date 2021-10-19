using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Saving;

namespace RPG.SceneManagement
{
    public class SavingWrapper : MonoBehaviour
    {
        // Constants
        const string PLAYER_PREFS_CURRENT_SAVE = "currentSave";

        public void ContinueGame()
        {
            if (!PlayerPrefs.HasKey(PLAYER_PREFS_CURRENT_SAVE)) { return; }
            if (!GetComponent<SavingSystem>().SaveFileExists(PLAYER_PREFS_CURRENT_SAVE)) { return; }
            StartCoroutine(LoadLastScene());
        }

        public void ContinueGame(string saveFile)
        {
            SetCurrentSave(saveFile);
            ContinueGame();
        }

        public void NewGame(string saveFile)
        {
            if (string.IsNullOrEmpty(saveFile)) { return; }

            SetCurrentSave(saveFile);
            StartCoroutine(LoadFirstScene(saveFile));
        }

        private void SetCurrentSave(string saveFile)
        {
            PlayerPrefs.SetString(PLAYER_PREFS_CURRENT_SAVE, saveFile);
        }

        private string GetCurrentSave()
        {
            return PlayerPrefs.GetString(PLAYER_PREFS_CURRENT_SAVE);
        }

        IEnumerator LoadFirstScene(string saveFile)
        {
            Fader fader = FindObjectOfType<Fader>();
            fader.ToggleFade(true);
            yield return fader.Fade();

            yield return GetComponent<SavingSystem>().LoadFirstScene();

            fader.ToggleFade(false);
            yield return fader.Fade();
            Save(saveFile);
        }

        IEnumerator LoadLastScene()
        {
            string currentSave = GetCurrentSave();
            if (string.IsNullOrEmpty(currentSave)) { yield break; }

            Fader fader = FindObjectOfType<Fader>();
            fader.ToggleFade(true);
            yield return fader.Fade();

            yield return GetComponent<SavingSystem>().LoadLastScene(currentSave);

            fader.ToggleFade(false);
            yield return fader.Fade();
        }

        private void Update()
        {
            //DebugMethods();
        }

        private void DebugMethods()
        {
            if (Input.GetKeyDown(KeyCode.L))
            {
                LoadFull();
            }
            if (Input.GetKeyDown(KeyCode.S))
            {
                Save();
            }
            if (Input.GetKeyDown(KeyCode.Delete))
            {
                Delete();
            }
        }

        public void Load()
        {
            GetComponent<SavingSystem>().Load(GetCurrentSave());
        }

        public void Load(string saveFile)
        {
            GetComponent<SavingSystem>().Load(saveFile);
        }

        private void LoadFull()
        {
            StartCoroutine(LoadLastScene());
        }

        public void Save()
        {
            GetComponent<SavingSystem>().Save(GetCurrentSave());
        }

        public void Save(string saveFile)
        {
            GetComponent<SavingSystem>().Save(saveFile);
        }

        public void Delete()
        {
            GetComponent<SavingSystem>().Delete(GetCurrentSave());
        }

        public void Delete(string saveFile)
        {
            GetComponent<SavingSystem>().Delete(saveFile);
        }
    }
}