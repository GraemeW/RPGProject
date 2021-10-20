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

        public bool ContinueGame()
        {
            if (!PlayerPrefs.HasKey(PLAYER_PREFS_CURRENT_SAVE)) { return false; }
            if (!GetComponent<SavingSystem>().SaveFileExists(PlayerPrefs.GetString(PLAYER_PREFS_CURRENT_SAVE))) { return false; }
            StartCoroutine(LoadLastScene());
            return true;
        }

        public bool ContinueGame(string saveFile)
        {
            SetCurrentSave(saveFile);
            if (ContinueGame()) { return true; }

            return false;
        }

        public void LoadMenu()
        {
            StartCoroutine(LoadMenuScene());
        }

        public void NewGame(string saveFile)
        {
            if (string.IsNullOrEmpty(saveFile)) { return; }

            if (GetComponent<SavingSystem>().SaveFileExists(PlayerPrefs.GetString(PLAYER_PREFS_CURRENT_SAVE))) { Delete(); }
            SetCurrentSave(saveFile);
            StartCoroutine(LoadFirstScene(saveFile));
        }

        private void SetCurrentSave(string saveFile)
        {
            PlayerPrefs.SetString(PLAYER_PREFS_CURRENT_SAVE, saveFile);
        }

        private string GetCurrentSave()
        {
            UnityEngine.Debug.Log(PlayerPrefs.GetString(PLAYER_PREFS_CURRENT_SAVE));
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

        public IEnumerator ReloadFirstScene(IEnumerable<Action> actionsAfterFade)
        {
            Fader fader = FindObjectOfType<Fader>();
            fader.ToggleFade(true);
            yield return fader.Fade();

            yield return GetComponent<SavingSystem>().LoadFirstScene(GetCurrentSave());
            foreach (Action action in actionsAfterFade)
            {
                action.Invoke();
            }

            fader.ToggleFade(false);
            yield return fader.Fade();
            Save();
        }

        IEnumerator LoadMenuScene()
        {
            Fader fader = FindObjectOfType<Fader>();
            fader.ToggleFade(true);
            yield return fader.Fade();

            yield return GetComponent<SavingSystem>().LoadMenuScene();

            fader.ToggleFade(false);
            yield return fader.Fade();
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

        public IEnumerable<string> ListSaves()
        {
            return GetComponent<SavingSystem>().ListSaves();
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