using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using RPG.Stats;

namespace RPG.UI.Traits
{
    public class TraitRowUI : MonoBehaviour
    {
        // Tunables
        [SerializeField] Trait trait = default;
        [SerializeField] TMP_Text valueText = null;
        [SerializeField] Button incrementButton = null;
        [SerializeField] Button decrementButton = null;

        // Cached References
        TraitStore playerTraitStore = null;

        private void Start()
        {
            SetupPlayerTraitStore();
            RefreshUI();
        }

        private void OnEnable()
        {
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

        public void Allocate(int points) // Called via Unity events
        {
            if (playerTraitStore == null) { return; }

            playerTraitStore.AssignPoints(trait, points);
        }

        private void RefreshUI()
        {
            if (playerTraitStore == null) { return; }

            decrementButton.interactable = playerTraitStore.CanAssignPoints(trait, -1);
            incrementButton.interactable = playerTraitStore.CanAssignPoints(trait, 1);
            valueText.text = playerTraitStore.GetProposedPoints(trait).ToString();
        }
    }
}
