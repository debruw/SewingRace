using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopeSpawn : MonoBehaviour
{
    [SerializeField]
    GameObject partPrefab, parentObject;
    GameObject firstObject, lastObject;

    [SerializeField]
    Transform player;

    Vector3 headPoint;

    [SerializeField]
    [Range(1, 1000)]
    int length = 1;

    [SerializeField]
    float partDistance = 0.21f;

    [SerializeField]
    bool reset, spawn, snapFirst, snapLast;

    bool delete;

   public int finalLength;
    public Color finalColor;

    // Update is called once per frame
    void Update()
    {
        if (reset)
        {
            foreach (GameObject tmp in GameObject.FindGameObjectsWithTag("Hair"))
            {
                Destroy(tmp);
            }

            reset = false;
        }

        if (spawn)
        {
            Spawn();

            spawn = false;
        }

        //if (GameManager.Instance.endGame)
        //{


        //    //    firstObject.transform.position = headPoint;
        //    //    firstObject.transform.rotation = new Quaternion(180, 0, 0, 0);

        //}
        //else
        //{
        //    headPoint = new Vector3(player.transform.position.x, player.transform.position.y + 1.38f, player.transform.position.z - 0.15f);

        //    firstObject.transform.position = headPoint;
        //}


    }


    public void Spawn()
    {
        int count = (int)(length / partDistance);

        for (int x = 0; x < length; x++)
        {

            GameObject tmp;

            tmp = Instantiate(partPrefab, new Vector3(transform.position.x, transform.position.y + partDistance * (x + 1), transform.position.z), Quaternion.identity, parentObject.transform);
            tmp.transform.eulerAngles = new Vector3(180, 0, 0);

            tmp.name = parentObject.transform.childCount.ToString();

            if (x == 0)
            {
                Destroy(tmp.GetComponent<CharacterJoint>());
                if (snapFirst)
                {
                    firstObject = tmp;
                    tmp.transform.eulerAngles = new Vector3(0, 0, 0);
                    tmp.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
                    tmp.GetComponent<Rigidbody>().constraints &= ~RigidbodyConstraints.FreezeRotationZ;
                }
            }
            else
            {
                tmp.GetComponent<CharacterJoint>().connectedBody = parentObject.transform.Find((parentObject.transform.childCount - 1).ToString()).GetComponent<Rigidbody>();
            }
        }

        if (snapLast)
            {
                //  parentObject.transform.Find((parentObject.transform.childCount).ToString()).GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
                parentObject.transform.Find((parentObject.transform.childCount).ToString()).GetComponent<Rigidbody>().drag = 35f;
            lastObject = parentObject.transform.Find((parentObject.transform.childCount).ToString()).gameObject;
            }
       
    }

    public void Delete()
    {
        delete = true;

        if (delete)
        {
            delete = false;
            finalLength = parentObject.transform.childCount;
            finalColor = parentObject.transform.GetChild(0).GetComponent<Renderer>().material.color;
            foreach (GameObject tmpd in GameObject.FindGameObjectsWithTag("Hair"))
            {
                Destroy(tmpd);
            }

            
            StartCoroutine("SpawnEnd");
        }

    }

    public IEnumerator SpawnEnd()
    {
        yield return new WaitForSeconds(0.15f);

        
        headPoint = new Vector3(player.transform.position.x, player.transform.position.y + -0.1f, player.transform.position.z + 0.75f);


        for (int x = 0; x < finalLength; x++)
        {

            GameObject tmp;

            tmp = Instantiate(partPrefab, new Vector3(headPoint.x, headPoint.y + partDistance * (x + 1), headPoint.z), Quaternion.identity, parentObject.transform);
            tmp.transform.eulerAngles = new Vector3(180, 0, 0);
            tmp.GetComponent<Renderer>().material.color = finalColor;

            tmp.name = parentObject.transform.childCount.ToString();

            if (x == 0)
            {
                Destroy(tmp.GetComponent<CharacterJoint>());
                if (snapFirst)
                {
                    firstObject = tmp;
                    tmp.transform.eulerAngles = new Vector3(0, 0, 0);
                    tmp.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
                    tmp.GetComponent<Rigidbody>().constraints &= ~RigidbodyConstraints.FreezeRotationZ;
                }
            }
            else
            {
                tmp.GetComponent<CharacterJoint>().connectedBody = parentObject.transform.Find((parentObject.transform.childCount - 1).ToString()).GetComponent<Rigidbody>();
            }
        }

        if (snapLast)
        {
            //  parentObject.transform.Find((parentObject.transform.childCount).ToString()).GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
            parentObject.transform.Find((parentObject.transform.childCount).ToString()).GetComponent<Rigidbody>().mass = 5f;
            lastObject = parentObject.transform.Find((parentObject.transform.childCount).ToString()).gameObject;
        }

    }
}
