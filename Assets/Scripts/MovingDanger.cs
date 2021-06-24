using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Obi;

public class MovingDanger : MonoBehaviour
{
    [SerializeField] bool isMoving, isTransparent;
    public Transform[] waypoints;
    int current = 0;
    float WPradius = .1f;
    public float speed;
    public MeshRenderer rend1, rend2;

    // Update is called once per frame
    void Update()
    {
        if (isTransparent)
        {
            if (transform.position.z - Camera.main.gameObject.transform.position.z < 8)
            {
                rend1.material.color = new Color(rend1.material.color.r, rend1.material.color.g, rend1.material.color.b, (transform.position.z - Camera.main.gameObject.transform.position.z) / 8);
                rend2.material.color = new Color(rend2.material.color.r, rend2.material.color.g, rend2.material.color.b, (transform.position.z - Camera.main.gameObject.transform.position.z) / 8);
            }
        }

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
