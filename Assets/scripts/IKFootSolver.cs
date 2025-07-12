using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IKFootSolver : MonoBehaviour
{
    [SerializeField] Transform body;             // Spider body reference
    public float stepDistance = 2f;              // Distance at which to trigger a step
    public Vector3 footOffset = Vector3.zero;    // Any vertical or lateral offset
    public LayerMask ground;                     // Ground detection layer

    Vector3 currentPosition;
    Vector3 newPosition;
    Vector3 oldPosition;

    bool isStepping;

    void Start()
    {
        currentPosition = transform.position;
        oldPosition = currentPosition;
    }

    void Update()
    {
        Vector3 rayOrigin = transform.position + Vector3.up * 2f;

        if (Physics.Raycast(rayOrigin, Vector3.down, out RaycastHit hit, 10f, ground))
        {
            Vector3 targetPoint = hit.point + footOffset;
            float distanceToTarget = Vector3.Distance(currentPosition, targetPoint);

            if (!isStepping && distanceToTarget > stepDistance)
            {
                isStepping = true;
                oldPosition = currentPosition;
                newPosition = targetPoint;
            }

            if (isStepping)
            {
                currentPosition = newPosition;
                transform.position = currentPosition;
                isStepping = false;
            }
            else
            {
                transform.position = currentPosition;
            }
        }

        Debug.DrawRay(rayOrigin, Vector3.down * 10f, Color.green);
    }
}
