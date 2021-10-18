using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using RPG.Stats;

namespace RPG.UI.Traits
{
    public class TraitUI : MonoBehaviour
    {
        // Tunables
        [SerializeField] TMP_Text unassignedPointsText = null;
        [SerializeField] Button commitButton = null;

        // Cached References
        TraitStore playerTraitStore = null;

        private void Start()
        {
            SetupPlayerTraitStore();
            RefreshUI();
        }

        private void OnDestroy()
        {
            if (playerTraitStore != null) { playerTraitStore.onChange -= RefreshUI; }
        }

        private void SetupPlayerTraitStore()
        {
            playerTraitStore = TraitStore.GetPlayerTraitStore();
            if (playerTraitStore == null) { return; }
            playerTraitStore.onChange += RefreshUI;
        }

        public void RefreshUI()
        {
            if (playerTraitStore == null) { return; }

            unassignedPointsText.text = playerTraitStore.GetUnassignedPoints().ToString();
            commitButton.interactable = playerTraitStore.HasStagedPoints();
        }

        public void CommitPlayerTraits() // Called by Unity Events
        {
            if (playerTraitStore == null) { return; }

            playerTraitStore.Commit();
        }
    }
}