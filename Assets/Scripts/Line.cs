using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Obi;

public class Line : MonoBehaviour
{
    [SerializeField] MeshRenderer netRenderer1, netRenderer2;
    float counter;
    [SerializeField] float LengthFactor = 1;
    [SerializeField] float ropeLength;
    PlayerControl playerControl;
    [SerializeField] Collider[] twoOther;
    bool isActivated;
    float range;

    private void Update()
    {
        if (isActivated && Input.GetMouseButton(0))
        {            
            if (GameManager.Instance.obiRopeManager.obiRopes[0].restLength > 0)
            {
                netRenderer1.material.SetFloat("_Fill", (Mathf.Abs(playerControl.transform.position.z) - Mathf.Abs(transform.position.z)) / range);
                netRenderer2.material.SetFloat("_Fill", (Mathf.Abs(playerControl.transform.position.z) - Mathf.Abs(transform.position.z)) / range);
                foreach (var item in GameManager.Instance.obiRopeManager.cursors)
                {
                    item.ChangeLength(item.GetComponent<ObiRope>().restLength - .02f);
                }
            }
            else
            {
                isActivated = false;
                GameManager.Instance.playerControl.m_animator.SetBool("Knitting", false);
            }

            if ((Mathf.Abs(playerControl.transform.position.z) - Mathf.Abs(transform.position.z)) / range > 1)
            {
                isActivated = false;
                GameManager.Instance.playerControl.m_animator.SetBool("Knitting", false);
            }
        }
        if (isActivated)
        {
            if (Input.GetMouseButton(0) && GameManager.Instance.playerControl.m_animator.GetBool("Knitting") == false)
            {
                GameManager.Instance.playerControl.m_animator.SetBool("Knitting", true);
            }
            else if (Input.GetMouseButtonUp(0))
            {
                GameManager.Instance.playerControl.m_animator.SetBool("Knitting", false);
            }
        }
        
    }

    public void OpenNet(PlayerControl pc)
    {
        netRenderer1.GetComponent<Collider>().enabled = true;
        //netRenderer2.GetComponent<Collider>().enabled = true;
        GetComponent<Collider>().enabled = false;
        foreach (Collider item in twoOther)
        {
            item.enabled = false;
        }
        netRenderer1.material.color = GameManager.Instance.obiRopeManager.solver.colors[0];
        netRenderer2.material.color = GameManager.Instance.obiRopeManager.solver.colors[1];
        playerControl = pc;
        range = Mathf.Abs(transform.position.z + netRenderer1.GetComponent<Collider>().bounds.size.z - 3) - Mathf.Abs(playerControl.transform.position.z);

        isActivated = true;
    }
}
