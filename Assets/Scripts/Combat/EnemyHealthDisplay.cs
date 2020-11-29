using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using RPG.Resources;

namespace RPG.Combat
{
    public class EnemyHealthDisplay : MonoBehaviour
    {
        // Tunables
        [SerializeField] TextMeshProUGUI enemyHealthValue = null;

        // Cached References
        Fighter playerFighter = null;
        Health enemyHealth = null;

        private void Start()
        {
            playerFighter = GameObject.FindGameObjectWithTag("Player").GetComponent<Fighter>();
        }

        private void Update()
        {
            UpdateTargetHealth();
        }

        private void UpdateTargetHealth()
        {
            enemyHealth = playerFighter.GetTarget();
            if (enemyHealth != null)
            {
                enemyHealthValue.text = enemyHealth.GetPercentage().ToString() + "%";
            }
            else
            {
                enemyHealthValue.text = "n/a";
            }
        }
    }
}
