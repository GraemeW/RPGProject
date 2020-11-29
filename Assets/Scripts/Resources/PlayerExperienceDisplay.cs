using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace RPG.Resources
{
    public class PlayerExperienceDisplay : MonoBehaviour
    {
        // Tunables
        [SerializeField] TextMeshProUGUI playerExperienceValue = null;

        // Cached References
        Experience playerExperience = null;

        private void Start()
        {
            playerExperience = GameObject.FindGameObjectWithTag("Player").GetComponent<Experience>();
        }

        private void Update()
        {
            playerExperienceValue.text = playerExperience.GetPercentage().ToString() + "%";
        }
    }
}