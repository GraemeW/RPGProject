using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace RPG.Abilities.Effects
{
    [CreateAssetMenu(fileName = "SpawnTargetPrefab Effect", menuName = "Abilities/Effects/SpawnTargetPrefab", order = 0)]
    public class SpawnTargetPrefabEffect : EffectStrategy
    {
        [SerializeField] GameObject prefabToSpawn = null;
        [SerializeField] float destroyDelay = -1;

        public override void StartEffect(AbilityData abilityData, Action finished)
        {
            abilityData.StartCoroutine(Effect(abilityData, finished));
        }

        private IEnumerator Effect(AbilityData abilityData, Action finished)
        {
            GameObject spawnedObject = Instantiate(prefabToSpawn);
            spawnedObject.transform.position = abilityData.GetTargetedPoint();

            if (destroyDelay > 0)
            {
                yield return new WaitForSeconds(destroyDelay);
                Destroy(spawnedObject);
            }
            finished.Invoke();
        }
    }
}
