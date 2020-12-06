using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using RPG.Combat;

namespace RPG.Resources
{
    public class PlayerHealthDisplay : MonoBehaviour
    {
        // Tunables
        [SerializeField] TextMeshProUGUI playerHealthValue = null;

        // Cached References
        Health playerHealth = null;

        private void Awake()
        {
            playerHealth = GameObject.FindGameObjectWithTag("Player").GetComponent<Health>();
        }

        private void Update()
        {
            playerHealthValue.text = string.Format("{0:0}/{1:0}", playerHealth.GetHealthPoints(), playerHealth.GetMaxHealthPoints());
        }
    }
}
