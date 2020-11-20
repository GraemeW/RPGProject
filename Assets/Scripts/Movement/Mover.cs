using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Mover : MonoBehaviour
{
    // Tunables

    // Cached References
    NavMeshAgent navMeshAgent = null;
    Animator animator = null;

    // State
    Ray lastRay;

    private void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (Input.GetButton("Fire1"))
        {
            MoveToCursor();
        }
        UpdateAnimator();
    }

    private void MoveToCursor()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        bool hasHit = Physics.Raycast(ray, out RaycastHit hitInfo);
        if (hasHit)
        {
            navMeshAgent.SetDestination(hitInfo.point);
        }
        lastRay = ray;
    }

    private void UpdateAnimator()
    {
        Vector3 velocity = navMeshAgent.velocity;
        Vector3 localVelocity = transform.InverseTransformDirection(velocity);
        animator.SetFloat("forwardSpeed", localVelocity.z);
    }
}
