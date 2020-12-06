﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Resources;
using RPG.Control;

namespace RPG.Combat
{
    [RequireComponent(typeof(Health))]
    public class CombatTarget : MonoBehaviour, IRaycastable
    {
        public bool HandleRaycast(PlayerController callingController, string interactButtonOne, string interactButtonTwo)
        {
            Fighter fighter = callingController.GetComponent<Fighter>();
            if (fighter == null) { return false; }
            if (!fighter.CanAttack(gameObject)) { return false; }

            if (Input.GetButtonDown(interactButtonTwo))
            {
                fighter.Attack(gameObject);
            }
            return true;
        }

        public CursorType GetCursorType()
        {
            return CursorType.Combat;
        }
    }
}
