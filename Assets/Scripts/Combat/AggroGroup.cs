using RPG.Control;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Combat
{
    public class AggroGroup : MonoBehaviour
    {
        [SerializeField] AIController[] aiControllers = null;
        [SerializeField] bool activateOnStart = false;

        private void Start()
        {
            if (activateOnStart) { Activate(true); }
        }

        public void Activate(bool shouldActivate)
        {
            foreach (AIController aiController in aiControllers)
            {
                aiController.SetFriendly(!shouldActivate);
            }
        }
    }
}