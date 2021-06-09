using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewYarn : MonoBehaviour
{
    public Color color;

    private void Start()
    {
        GetComponent<MeshRenderer>().material.color = color;
    }
}
