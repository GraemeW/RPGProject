using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace RPG.UI.DamageText
{
    public class DamageText : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI damageTextElement = null;

        public void DestroyText()
        {
            Destroy(gameObject);
        }

        public void SetText(float damageAmount)
        {
            damageTextElement.text = Mathf.RoundToInt(damageAmount).ToString();
        }

        public void SetColor(Color color)
        {
            damageTextElement.color = color;
        }
    }
}
