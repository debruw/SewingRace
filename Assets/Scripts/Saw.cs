using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Saw : MonoBehaviour
{
    Transform parent;

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
                    parent.transform.GetChild(i).GetComponent<Rigidbody>().drag = 35f;
                }
                else
                {
                    //Destroy cut hair
                    GameObject child = parent.GetChild(i).gameObject;
                    Destroy(child);
                }

            }
        }
    }
}
