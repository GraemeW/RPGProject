using RPG.Quests;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.UI.Quests
{
    public class QuestListUI : MonoBehaviour
    {
        // Tunables
        [SerializeField] QuestItemUI questPrefab;

        // Cached References
        QuestList playerQuestList = null;

        private void Awake()
        {
            playerQuestList = GameObject.FindGameObjectWithTag("Player").GetComponent<QuestList>();
        }

        private void OnEnable()
        {
            playerQuestList.questListUpdated += PaintQuests;
            PaintQuests();
        }

        private void OnDisable()
        {
            playerQuestList.questListUpdated -= PaintQuests;
        }

        private void PaintQuests()
        {
            ClearQuests();
            foreach (QuestStatus questStatus in playerQuestList.GetActiveQuests())
            {
                QuestItemUI uiInstance = Instantiate(questPrefab, transform);
                uiInstance.SetUp(questStatus);
            }
        }

        private void ClearQuests()
        {
            foreach (Transform child in transform)
            {
                Destroy(child.gameObject);
            }
        }
    }
}
