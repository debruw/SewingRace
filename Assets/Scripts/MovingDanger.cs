using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Hair")
        {
            parent = other.gameObject.transform.parent;
            int hitObject = int.Parse(other.name);

            for (int i = hitObject; i < parent.childCount; i++)
            {
                if (i == hitObject)
                {
                    //Give mass
                    parent.transform.GetChild(i).GetComponent<Rigidbody>().velocity = Vector3.down;
                    parent.transform.GetChild(i).GetComponent<CharacterJoint>().connectedBody = null;
                }
                else
                {
                    //Destroy cut rope
                    GameObject child = parent.GetChild(i).gameObject;
                    GameManager.Instance.ropeManager.RopeParts.Remove(child);
                    Destroy(child, .5f);
                }
            }

        }
    }
}
