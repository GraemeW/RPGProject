using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace RPG.Attributes.UI
{
    public class PlayerManaDisplay : MonoBehaviour
    {
        // Tunables
        [SerializeField] TMP_Text playerManaValue = null;

        // Cached References
        Mana playerMana = null;

        private void Awake()
        {
            playerMana = GameObject.FindGameObjectWithTag("Player").GetComponent<Mana>();
        }

        private void Update()
        {
            playerManaValue.text = string.Format("{0:0}/{1:0}", playerMana.GetMana(), playerMana.GetMaxMana());
        }
    }

}
