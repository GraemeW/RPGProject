using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace RPG.Inventories
{
    public class RandomDropper : ItemDropper
    {
        // Tunables
        [Header("Drop Scatter Properties")]
        [SerializeField] float minDropOffset = 1f;
        [SerializeField] float maxDropOffset = 3.0f;
        [SerializeField] float dropHeight = 0.5f;
        [SerializeField] bool checkForReachable = false;
        [SerializeField] float navmeshSampledDistance = 0.7f;
        [Header("Death Drop Properties")]
        [SerializeField] InventoryItem[] dropLibrary;
        [SerializeField] int numberOfDrops = 2;

        // Constants
        const int MAX_ATTEMPTS = 10;

        public void RandomDrop()
        {
            if (dropLibrary == null) { return; }
            for (int dropIndex = 0; dropIndex < numberOfDrops; dropIndex++)
            {
                int itemIndex = Random.Range(0, dropLibrary.Length);
                DropItem(dropLibrary[itemIndex]);
            }
        }

        protected override Vector3 GetDropLocation()
        {
            if (checkForReachable)
            {
                return GetReachableDropLocation();
            }
            else
            {
                return GetAnyDropLocation();
            }
        }

        private Vector3 GetAnyDropLocation()
        {
            GetRandomOffset(out float xOffset, out float zOffset);
            Vector3 dropLocation = transform.position + transform.right * xOffset + transform.forward * zOffset + transform.up * maxDropOffset * dropHeight;
            return dropLocation;
        }
        
        private Vector3 GetReachableDropLocation()
        {
            for (int attemptIndex = 0; attemptIndex < MAX_ATTEMPTS; attemptIndex++)
            {
                GetRandomOffset(out float xOffset, out float zOffset);
                Vector3 dropLocation = transform.position + transform.right * xOffset + transform.forward * zOffset + transform.up * maxDropOffset * dropHeight;
                if (CheckDropPositionReachable(dropLocation))
                {
                    return dropLocation;
                }
            }
            return transform.position;
        }

        private bool CheckDropPositionReachable(Vector3 dropLocation)
        {
            if (NavMesh.SamplePosition(dropLocation, out NavMeshHit hit, navmeshSampledDistance, NavMesh.AllAreas))
            {
                return true;
            }
            return false;
        }

        private void GetRandomOffset(out float xOffset, out float zOffset)
        {
            float angle = Random.Range(0f, 2*Mathf.PI);
            float magnitude = Random.Range(minDropOffset, maxDropOffset);
            xOffset = Mathf.Cos(angle) * magnitude;
            zOffset = Mathf.Sin(angle) * magnitude;
        }
    }
}
