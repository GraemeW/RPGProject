using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace RPG.UI.Quests
{
    public class QuestObjectiveUI : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI detail = null;
        [SerializeField] GameObject ProgressIndicatorComplete = null;

        public void Setup(string detailText, bool isComplete)
        {
            detail.text = detailText;
            SetProgress(isComplete);
        }

        public void SetProgress(bool isComplete)
        {
            ProgressIndicatorComplete.SetActive(isComplete);
            if (isComplete) { detail.color = Color.green; }
            else { detail.color = Color.white; }
        }
    }
}