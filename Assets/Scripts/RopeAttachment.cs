using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopeAttachment : MonoBehaviour
{
    public Transform target;
    public Vector3 Offset;
    public float SmoothTime = 0.01f;
    private Vector3 velocity = Vector3.zero;

    // Start is called before the first frame update
    void Start()
    {
        Offset = transform.position - target.position;
    }

    void LateUpdate()
    {
        Vector3 targetPosition = target.position + Offset;
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, SmoothTime);
    }
}
