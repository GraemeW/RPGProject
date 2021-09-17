using RPG.Control;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Abilities
{
    [CreateAssetMenu(fileName = "Demo Targeting", menuName = "Abilities/Targeting/Delayed Click")]
    public class DelayedClickTargeting : TargetingStrategy
    {
        [SerializeField] Texture2D cursorTexture = null;
        [SerializeField] Vector2 cursorHotspot = new Vector2();

        public override void StartTargeting(GameObject user)
        {
            if (!user.TryGetComponent<PlayerController>(out PlayerController playerController)) { return; }

            playerController.StartCoroutine(Targeting(playerController));
        }

        private IEnumerator Targeting(PlayerController playerController)
        {
            playerController.enabled = false;
            while(true)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    // Absorb the whole mouse click
                    yield return new WaitWhile(() => Input.GetMouseButton(0));
                    playerController.enabled = true;
                    yield break;
                }

                Cursor.SetCursor(cursorTexture, cursorHotspot, CursorMode.Auto);
                yield return null;
            }
        }
    }

}
