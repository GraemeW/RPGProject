using RPG.Inventories;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


namespace RPG.UI.Inventories
{
    public class PurseUI : MonoBehaviour
    {
        // Tunables
        [SerializeField] TMP_Text purseField = null;

        // State
        Purse purse = null;

        private void Start()
        {
            SetupPlayerPurse();
            RefreshUI();
        }

        private void OnDestroy()
        {
            if (purse != null) { purse.onChange -= RefreshUI; }
        }

        private void SetupPlayerPurse()
        {
            purse = Purse.GetPlayerPurse();
            if (purse == null) { return; }
            purse.onChange += RefreshUI;
        }

        private void RefreshUI()
        {
            if (purse == null) { return; }

            purseField.text = $"${purse.GetBalance():N2}";
        }
    }

}
