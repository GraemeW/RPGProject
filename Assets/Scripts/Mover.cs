using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Mover : MonoBehaviour
{
    // Tunables
    [SerializeField] Transform target = null;

    // Cached References
    NavMeshAgent navMeshAgent = null;

    private void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        navMeshAgent.SetDestination(target.position);
    }
}
