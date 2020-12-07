using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.UI.DamageText
{
    public class DamageTextSpawner : MonoBehaviour
    {
        [SerializeField] Color damageTextColor = Color.red;
        [SerializeField] DamageText damageTextPrefab = null;

        public void Spawn(float damageAmount)
        {
            DamageText damageTextInstance = Instantiate(damageTextPrefab, transform);
            damageTextInstance.SetText(damageAmount);
            damageTextInstance.SetColor(damageTextColor);
        }
    }
}