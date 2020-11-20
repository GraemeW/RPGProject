using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    [SerializeField] Transform target = null;

    private void LateUpdate()
    {
        transform.position = target.position;
    }
}
