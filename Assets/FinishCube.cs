using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishCube : MonoBehaviour
{
    public MeshRenderer meshRenderer1, meshRenderer2;
    Color color1;

    private void Start()
    {
        color1 = meshRenderer1.material.color;
    }

    public void ActivateEffect()
    {
        StartCoroutine(WaitAndChangecolor());
    }

    bool state;
    IEnumerator WaitAndChangecolor()
    {
        if (state)
        {
            meshRenderer1.material.color = Color.white;
            meshRenderer2.material.color = Color.white; 
        }
        else
        {
            meshRenderer1.material.color = color1;
            meshRenderer2.material.color = color1;
        }
        state = !state;
        yield return new WaitForSeconds(.5f);
        StartCoroutine(WaitAndChangecolor());
    }
}
