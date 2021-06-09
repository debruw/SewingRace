using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destroyer : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Saw"))
        {
            Destroy(other.transform.parent.gameObject);
        }
    }
}
