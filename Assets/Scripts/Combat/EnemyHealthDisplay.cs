using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using RPG.Attributes;

namespace RPG.Combat
{
    public class EnemyHealthDisplay : MonoBehaviour
    {
        // Tunables
        [SerializeField] TextMeshProUGUI enemyHealthValue = null;

        // Cached References
        Fighter playerFighter = null;
        Health enemyHealth = null;

        private void Awake()
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
                enemyHealthValue.text = string.Format("{0:0}/{1:0}", enemyHealth.GetHealthPoints(), enemyHealth.GetMaxPoints());
            }
            else
            {
                enemyHealthValue.text = "n/a";
            }
        }
    }
}
