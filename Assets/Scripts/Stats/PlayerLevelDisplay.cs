using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace RPG.Stats
{
    public class PlayerLevelDisplay : MonoBehaviour
    {
        // Tunables
        [SerializeField] TextMeshProUGUI playerLevelValue = null;

        // Cached References
        BaseStats baseStats = null;

        private void Awake()
        {
            baseStats = GameObject.FindGameObjectWithTag("Player").GetComponent<BaseStats>();
        }

        private void Update()
        {
            playerLevelValue.text = baseStats.GetLevel().ToString();
        }
    }
}