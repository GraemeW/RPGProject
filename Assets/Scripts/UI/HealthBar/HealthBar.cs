using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using RPG.Attributes;

namespace RPG.UI.HealthBar
{
    public class HealthBar : MonoBehaviour
    {
        [SerializeField] Image healthBarForeground = null;
        [SerializeField] Canvas canvas = null;

        // State
        Health health = null;
        Vector3 barScale = new Vector3();

        private void Awake()
        {
            health = GetComponentInParent<Health>();
        }

        private void Start()
        {
            barScale = healthBarForeground.transform.localScale;
        }

        private void Update()
        {
            if (health.IsDead()) { gameObject.SetActive(false); }
            if (health.isMaxHealth()) { canvas.enabled = false; }
            else { canvas.enabled = true; }
            UpdateForegroundFraction();
        }

        private void UpdateForegroundFraction()
        {
            Vector3 barScaleUpdate = new Vector3(Mathf.Clamp(health.GetFraction(), 0f, 100f), barScale.y, barScale.y);
            healthBarForeground.transform.localScale = barScaleUpdate;
        }
    }
}