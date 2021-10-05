using RPG.Control;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Abilities
{
    [CreateAssetMenu(fileName = "Delayed Click Targeting", menuName = "Abilities/Targeting/DelayedClick")]
    public class DelayedClickTargeting : TargetingStrategy
    {
        // Tunables
        [SerializeField] Texture2D cursorTexture = null;
        [SerializeField] Vector2 cursorHotspot = new Vector2();
        [SerializeField] LayerMask layerMask = new LayerMask();
        [SerializeField] float areaOfEffectRadius = 5f;
        [SerializeField] GameObject targetingGraphicPrefab = null;

        // State
        GameObject targetingGraphic = null;

        public override void StartTargeting(AbilityData abilityData, Action finished)
        {
            if (!abilityData.GetUser().TryGetComponent<PlayerController>(out PlayerController playerController)) { return; }

            playerController.StartCoroutine(Targeting(playerController, abilityData, finished));
        }

        private IEnumerator Targeting(PlayerController playerController, AbilityData abilityData, Action finished)
        {
            playerController.enabled = false;

            if (targetingGraphic != null) { Destroy(targetingGraphic); }
            targetingGraphic = Instantiate(targetingGraphicPrefab);
            targetingGraphic.transform.localScale = new Vector3(areaOfEffectRadius * 2, 1f, areaOfEffectRadius * 2);

            while(!abilityData.IsCancelled())
            {
                if (Physics.Raycast(PlayerController.GetMouseRay(), out RaycastHit raycastHit, Mathf.Infinity, layerMask))
                {
                    if (Input.GetMouseButtonDown(0))
                    {
                        // Absorb the whole mouse click
                        yield return new WaitWhile(() => Input.GetMouseButton(0));
                        abilityData.SetTargetedPoint(raycastHit.point);
                        abilityData.SetTargets(GetGameObjectsInRadius(raycastHit.point));
                        Destroy(targetingGraphic);
                        break;
                    }
                    targetingGraphic.SetActive(true);
                    targetingGraphic.transform.position = raycastHit.point;
                }
                else
                {
                    targetingGraphic.SetActive(false);
                }
                Cursor.SetCursor(cursorTexture, cursorHotspot, CursorMode.Auto);
                yield return null;
            }

            if (targetingGraphic != null) { Destroy(targetingGraphic); }
            playerController.enabled = true;
            finished.Invoke();
        }

        private IEnumerable<GameObject> GetGameObjectsInRadius(Vector3 point)
        {
            RaycastHit[] hitsInfo = PlayerController.RaycastAllSorted(areaOfEffectRadius, point);
            foreach (RaycastHit hit in hitsInfo)
            {
                yield return hit.collider.gameObject;
            }
        }
    }

}
