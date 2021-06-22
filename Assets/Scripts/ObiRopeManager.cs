using Obi;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ObiRopeManager : MonoBehaviour
{
    //public ObiRope rope;

    public ObiSolver solver;

    public float Distance = .1f;
    public float Damper = 500f;

    public ObiRopeCursor[] cursors;
    public GameObject yarnBall, stick1, stick2;
    public ObiRope[] obiRopes;

    //int lastIndice;
    //float magnitude = 100;
    // Update is called once per frame
    void Update()
    {
        foreach (ObiRope item in obiRopes)
        {
            for (int i = 1; i < item.activeParticleCount; ++i)
            {
                int solverIndex = item.solverIndices[i];
                int prevSolverIndex = item.solverIndices[i - 1];

                Vector3 connection = solver.velocities[solverIndex] - solver.velocities[prevSolverIndex];

                var velocityTarget = connection + ((Vector3)solver.velocities[solverIndex] + Physics.gravity);
                var projectOnConnection = Vector3.Project(velocityTarget, connection);
                solver.velocities[solverIndex] = (velocityTarget - projectOnConnection) / (1 + Damper * Time.fixedDeltaTime);

                solver.velocities[solverIndex] = new Vector3(solver.velocities[solverIndex].x, Mathf.Clamp(solver.velocities[solverIndex].y, float.MinValue, -3), Mathf.Clamp(solver.velocities[solverIndex].z, float.MinValue, -2));
            }
        }

        //for (int i = 0; i < solver.positions.count; i++)
        //{
        //    if (solver.positions[i].magnitude != 0)
        //    {
        //        if (solver.positions[i].magnitude < magnitude)
        //        {
        //            magnitude = solver.positions[i].magnitude;
        //            lastIndice = i;
        //        }
        //    }
        //}
        //puskuls[0].transform.localPosition = solver.positions[obiRopes[0].solverIndices[obiRopes[0].elements.Count - 1]];
    }

    public float addingRopeLenght;
    public Color LastColor;
    public void AddNewRope(Color clr)
    {
        if (yarnBall.transform.localScale.z < 38)
        {
            yarnBall.transform.DOScale(yarnBall.transform.localScale + new Vector3(2, 2, 2), .3f);
        }
        foreach (ObiRope item in obiRopes)
        {
            for (int i = 0; i < item.activeParticleCount; i++)
            {
                solver.colors[i] = Color.Lerp(LastColor, clr, (float)i / (float)item.activeParticleCount);
                //solver.colors[i + 100] = Color.Lerp(LastColor, clr, (float)i / (float)item.activeParticleCount);
                //solver.colors[i + 200] = Color.Lerp(LastColor, clr, (float)i / (float)item.activeParticleCount);
            }
        }
        yarnBall.GetComponent<MeshRenderer>().material.DOColor(clr, .3f);

        foreach (ObiRopeCursor item in cursors)
        {
            item.ChangeLength(item.GetComponent<ObiRope>().restLength + addingRopeLenght);
            item.GetComponent<ObiRope>().RebuildConstraintsFromElements();
            item.UpdateCursor();
            item.UpdateSource();
        }
        LastColor = clr;
    }

    public IEnumerator RemoveRope(float ropeLength)
    {
        if (yarnBall.transform.localScale.x > 26)
        {
            yarnBall.transform.DOScale((yarnBall.transform.localScale - new Vector3(2, 2, 2)), .3f);
        }

        while (ropeLength > 0)
        {
            foreach (ObiRopeCursor item in cursors)
            {
                item.ChangeLength(GetComponent<ObiRope>().restLength - .03f);
                item.GetComponent<ObiRope>().RebuildConstraintsFromElements();
            }
            ropeLength -= .03f;
            if (obiRopes[0].restLength <= 0)
            {
                if (!GameManager.Instance.isScalingRope)
                {
                    GameManager.Instance.isGameOver = true;
                    //LOSE
                    StartCoroutine(GameManager.Instance.WaitAndGameLose());
                    Debug.Log(obiRopes[1].restLength);
                    GameManager.Instance.obiRopeManager.CloseYarnThings();
                    GameManager.Instance.playerControl.m_animator.SetTrigger("Lose");
                }
                break;
            }
            yield return new WaitForSeconds(0);
        }
    }

    public void CutRope(int particleIndex)
    {

        foreach (ObiRope item in obiRopes)
        {
            ObiSolver.ParticleInActor pa = solver.particleToActor[particleIndex];

            foreach (ObiRopeCursor cr in cursors)
            {
                cr.cursorMu = (float)pa.indexInActor / (float)item.activeParticleCount;
                cr.UpdateCursor();
            }

            item.Tear(item.elements[pa.indexInActor]);

            item.RebuildConstraintsFromElements();
        }

    }

    //public GameObject[] puskuls;
    public void CloseYarnThings()
    {
        foreach (ObiRope item in obiRopes)
        {
            item.gameObject.SetActive(false);
        }
        yarnBall.SetActive(false);
        //foreach (var item in puskuls)
        //{
        //    item.SetActive(false);
        //}
        stick1.SetActive(false);
        stick2.SetActive(false);
    }
}
