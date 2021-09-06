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
            SetupPlayerPurse(true);
            RefreshUI();
        }

        private void OnEnable()
        {
            SetupPlayerPurse(true);
        }

        private void OnDisable()
        {
            SetupPlayerPurse(false);
        }

        private void SetupPlayerPurse(bool enable)
        {
            if (enable & purse == null)
            {
                purse = Purse.GetPlayerPurse();
                if (purse == null) { return; }

                purse.onChange += RefreshUI;
            }
            else if (!enable && purse != null)
            {
                purse.onChange -= RefreshUI;
            }
        }

        private void RefreshUI()
        {
            if (purse == null) { return; }

            purseField.text = $"${purse.GetBalance():N2}";
        }
    }

}
