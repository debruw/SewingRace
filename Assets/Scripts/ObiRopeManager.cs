using Obi;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TapticPlugin;

public class ObiRopeManager : MonoBehaviour
{
    //public ObiRope rope;

    public ObiSolver solver;

    public float Distance = .1f;
    public float Damper = 500f;

    public ObiRopeCursor[] cursors;
    public GameObject yarnBall, stick1, stick2;
    public ObiRope obiRope;

    void Start()
    {
        obiRope.OnRopeTorn += DestroyTeared_OnRopeTorn;
    }

    // Update is called once per frame
    void Update()
    {

        for (int i = 1; i < obiRope.activeParticleCount; ++i)
        {
            int solverIndex = obiRope.solverIndices[i];
            int prevSolverIndex = obiRope.solverIndices[i - 1];

            Vector3 connection = solver.velocities[solverIndex] - solver.velocities[prevSolverIndex];

            var velocityTarget = connection + ((Vector3)solver.velocities[solverIndex] + Physics.gravity);
            var projectOnConnection = Vector3.Project(velocityTarget, connection);
            solver.velocities[solverIndex] = (velocityTarget - projectOnConnection) / (1 + Damper * Time.fixedDeltaTime);

            solver.velocities[solverIndex] = new Vector3(solver.velocities[solverIndex].x, Mathf.Clamp(solver.velocities[solverIndex].y, float.MinValue, -3), Mathf.Clamp(solver.velocities[solverIndex].z, float.MinValue, -2));
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

        for (int i = 0; i < obiRope.activeParticleCount; i++)
        {
            solver.colors[i] = Color.Lerp(clr, LastColor, (float)i / (float)obiRope.activeParticleCount);
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
                item.UpdateCursor();
                item.UpdateSource();
            }
            ropeLength -= .03f;
            if (obiRope.restLength <= 0)
            {
                if (!GameManager.Instance.isScalingRope)
                {
                    GameManager.Instance.isGameOver = true;
                    //LOSE
                    StartCoroutine(GameManager.Instance.WaitAndGameLose());
                    Debug.Log(obiRope.restLength);
                    GameManager.Instance.obiRopeManager.CloseYarnThings();
                    GameManager.Instance.playerControl.m_animator.SetTrigger("Lose");
                }
                break;
            }
            yield return new WaitForSeconds(0);
        }
    }

    float distance = 50;
    int index;
    public bool isCutted;
    public void CutRope(int particleIndex)
    {
        Debug.Log("Cut Rope");
        isCutted = true;

        ObiSolver.ParticleInActor pa = solver.particleToActor[particleIndex];

        obiRope.Tear(obiRope.elements[pa.indexInActor]);
        obiRope.RebuildConstraintsFromElements();

        foreach (ObiRopeCursor cr in cursors)
        {
            cr.cursorMu = (float)pa.indexInActor / (float)obiRope.activeParticleCount;
            cr.UpdateCursor();
        }

        SoundManager.Instance.playSound(SoundManager.GameSounds.Cut);
        if (PlayerPrefs.GetInt("VIBRATION") == 1)
            TapticManager.Impact(ImpactFeedback.Medium);

    }

    private void DestroyTeared_OnRopeTorn(ObiRope rope, ObiRope.ObiRopeTornEventArgs tearInfo)
    {
        for (int i = rope.elements.Count - 1; i >= 0; --i)
        {
            var elm = rope.elements[i];

            rope.DeactivateParticle(rope.solver.particleToActor[elm.particle2].indexInActor);
            rope.elements.RemoveAt(i);

            if (elm == tearInfo.element)
                break;
        }

        rope.RebuildConstraintsFromElements();
    }

    //public GameObject[] puskuls;
    public void CloseYarnThings()
    {
        obiRope.GetComponent<MeshRenderer>().enabled = false;

        yarnBall.SetActive(false);
        //foreach (var item in puskuls)
        //{
        //    item.SetActive(false);
        //}
        stick1.SetActive(false);
        stick2.SetActive(false);
    }
}
