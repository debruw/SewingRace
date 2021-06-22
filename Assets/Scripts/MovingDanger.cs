using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Obi;

public class MovingDanger : MonoBehaviour
{
    [SerializeField] bool isMoving;
    public Transform[] waypoints;
    int current = 0;
    float WPradius = .1f;
    public float speed;
    Transform parent;

    // Update is called once per frame
    void Update()
    {
        if (!isMoving)
        {
            return;
        }
        if (Mathf.Abs(waypoints[current].position.x - transform.position.x) < WPradius)
        {
            current = Random.Range(0, waypoints.Length);
            if (current >= waypoints.Length)
            {
                current = 0;
            }
        }
        transform.position = Vector3.MoveTowards(transform.position, waypoints[current].position, Time.deltaTime * speed);
    }
}
