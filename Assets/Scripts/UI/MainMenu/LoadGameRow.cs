using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Events;

namespace RPG.UI
{
    public class LoadGameRow : MonoBehaviour
    {
        // Tunables
        [SerializeField] TMP_Text gameNameField = null;
        [SerializeField] Button loadGameButton = null;

        // State
        string saveFile = null;

        public void Setup(string saveFile, UnityEvent<string> unityEvent)
        {
            if (string.IsNullOrEmpty(saveFile)) { Destroy(gameObject); }
            if (unityEvent == null || unityEvent.GetPersistentEventCount() == 0) { Destroy(gameObject); }

            this.saveFile = saveFile;
            gameNameField.text = saveFile;

            loadGameButton.onClick.AddListener(() => unityEvent.Invoke(saveFile));
        }

        public string GetSaveFile()
        {
            return saveFile;
        }
    }
}
