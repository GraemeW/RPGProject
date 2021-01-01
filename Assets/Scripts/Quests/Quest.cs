using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Quests
{
    [CreateAssetMenu(fileName = "Quest", menuName = "Quests/New Quest")]
    public class Quest : ScriptableObject
    {
        [SerializeField] string detail = "";
        [SerializeField] string[] objectives = null;

        public string GetDetail()
        {
            return detail;
        }

        public int GetObjectiveCount()
        {
            return objectives.Length;
        }

        public string[] GetObjectiveDetails()
        {
            return objectives;
        }
    }
}