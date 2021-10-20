using RPG.SceneManagement;
using RPG.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace RPG.UI
{
    public class SaveLoadUI : MonoBehaviour
    {
        // Tunables
        [SerializeField] GameObject parentGameObject = null;
        [SerializeField] LoadGameRow childToSpawn = null;
        [SerializeField] UnityEvent<string> unityEvent = null;

        // State
        LazyValue<SavingWrapper> savingWrapper;

        private void Awake()
        {
            savingWrapper = new LazyValue<SavingWrapper>(GetSavingWrapper);
        }

        private SavingWrapper GetSavingWrapper()
        {
            return FindObjectOfType<SavingWrapper>();
        }

        private void Start()
        {
            savingWrapper.ForceInit();
        }

        private void OnEnable()
        {
            ResetUI();
        }

        private void ResetUI()
        {
            foreach (Transform child in parentGameObject.transform)
            {
                Destroy(child.gameObject);
            }

            if (savingWrapper.value == null) { return; }
            foreach(string saveFile in savingWrapper.value.ListSaves())
            {
                LoadGameRow loadGameRow = Instantiate(childToSpawn, parentGameObject.transform);
                loadGameRow.Setup(saveFile, unityEvent);
            }

        }
    }

}
