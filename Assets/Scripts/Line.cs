using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Line : MonoBehaviour
{
    [SerializeField] MeshRenderer netRenderer;
    float counter;
    [SerializeField] float LengthFactor = 1;
    [SerializeField] int ropeLength;
    [HideInInspector]
    public PlayerControl playerControl;
    [SerializeField] Collider[] twoOther;

    public void OpenNet()
    {
        netRenderer.GetComponent<Collider>().enabled = true;
        foreach (Collider item in twoOther)
        {
            item.enabled = false;
        }
        StartCoroutine(GameManager.Instance.ropeManager.RemoveRope(ropeLength));        
        StartCoroutine(WaitAndFillNet());
    }

    IEnumerator WaitAndFillNet()
    {
        if (counter < 1)
        {
            counter += (Time.deltaTime * LengthFactor);
            netRenderer.material.SetFloat("_Fill", counter);
            yield return new WaitForSeconds(.005f / LengthFactor);
            StartCoroutine(WaitAndFillNet());
        }
    }
}
