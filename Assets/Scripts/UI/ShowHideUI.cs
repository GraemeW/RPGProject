using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace RPG.UI
{
    public class ShowHideUI : MonoBehaviour
    {
        [SerializeField] KeyCode toggleKey = KeyCode.Escape;
        [SerializeField] GameObject uiContainer = null;
        [SerializeField] UnityEvent additionalEventCalls = null;

        void Start()
        {
            uiContainer.SetActive(false);
        }

        void Update()
        {
            if (Input.GetKeyDown(toggleKey))
            {
                Toggle();
            }
        }

        public void Toggle()
        {
            bool enable = !uiContainer.activeSelf;
            uiContainer.SetActive(enable);

            if (enable == true && additionalEventCalls != null)
            {
                additionalEventCalls.Invoke();
            }
        }
    }
}