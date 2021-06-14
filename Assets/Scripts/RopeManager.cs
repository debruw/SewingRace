using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopeManager : MonoBehaviour
{
    public List<GameObject> RopeParts;

    [SerializeField]
    int addHair = 8;
    [SerializeField]
    GameObject partPrefab;

    [SerializeField]
    float partDistance = 0.1f;
    [SerializeField]
    Transform PlayerTransform;

    public void AddNewRope(Collider other, Color clr)
    {
        Color lastColor = RopeParts[RopeParts.Count - 1].GetComponent<Renderer>().material.color;
        float colorLerp = .4f;

        for (int i = 0; i <= addHair; i++)
        {
            if (i == 0)
            {
                RopeParts[RopeParts.Count - 1].GetComponent<Rigidbody>().drag = 0f;
            }
            else
            {
                GameObject tmp;

                Vector3 lastOne = RopeParts[RopeParts.Count - 1].transform.position;
                RopeParts[RopeParts.Count - 1].transform.eulerAngles = new Vector3(90, 0, 0);

                tmp = Instantiate(partPrefab, new Vector3(lastOne.x, lastOne.y, lastOne.z - partDistance), Quaternion.identity, transform);
                tmp.transform.eulerAngles = new Vector3(90, 0, 0);

                tmp.name = (RopeParts.Count + 1).ToString();

                if (lastColor != clr && i < 3)
                {
                    tmp.GetComponent<Renderer>().material.color
                        = Color.Lerp(tmp.GetComponent<Renderer>().material.color, lastColor, colorLerp);
                    colorLerp += .2f;
                }
                else
                {
                    tmp.GetComponent<Renderer>().material.color = clr;
                }

                //tmp.GetComponent<CharacterJoint>().connectedBody = RopeParts[RopeParts.Count - 1].GetComponent<Rigidbody>();
                tmp.GetComponent<DistanceJoint3D>().ConnectedRigidbody = RopeParts[RopeParts.Count - 1].transform;

                RopeParts.Add(tmp);
            }
            if (i == addHair)
            {
                RopeParts[RopeParts.Count - 1].GetComponent<Rigidbody>().drag = 35f;
            }
        }

        //LastFixedPoint.GetComponent<CharacterJoint>().connectedBody = RopeParts[RopeParts.Count - 1].GetComponent<Rigidbody>();
    }

    public IEnumerator RemoveRope(int ropeLength)
    {
        while (ropeLength > 0)
        {
            Destroy(RopeParts[RopeParts.Count - 1], .1f);
            RopeParts.Remove(RopeParts[RopeParts.Count - 1]);
            ropeLength--;
            yield return new WaitForSeconds(.1f);
        }
        //LastFixedPoint.GetComponent<CharacterJoint>().connectedBody = RopeParts[RopeParts.Count - 1].GetComponent<Rigidbody>();
    }
}
