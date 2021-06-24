using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Obi;
using DG.Tweening;

public class PlayerControl : MonoBehaviour
{
    [SerializeField] float xSpeed, zSpeed;

    public CharacterController controller;
    public GameObject cam;

    public float speed = 12f;
    public float gravity = -9.81f;
    public float jumpHeight = 3f;

    public Transform groundCheck;
    public float groundDistance = 0.3f;
    public LayerMask groundMask;

    Vector3 velocity;

    bool isGrounded;
    public Animator m_animator;
    Touch touch;
    public Transform FinishTransform;
    float totalRoadLength;

    private void Start()
    {
        totalRoadLength = FinishTransform.position.z - transform.position.z;
    }

    private void Update()
    {
        if (!GameManager.Instance.isGameStarted)
        {
            if (Input.GetMouseButtonDown(0))
            {
                GameManager.Instance.TapToStartButtonClick();
                m_animator.SetBool("Walking", true);
            }
            return;
        }
        if (!GameManager.Instance.isGameStarted || GameManager.Instance.isGameOver)
        {
            return;
        }
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -5f;
        }
#if UNITY_EDITOR
        if (Input.GetMouseButton(0))
        {
            Vector3 move = Vector3.right * Input.GetAxis("Mouse X") * xSpeed + Vector3.forward * zSpeed;

            controller.Move(move * speed * Time.deltaTime);

            if (GameManager.Instance.isScalingRope)
            {
                foreach (ObiRopeCursor item in GameManager.Instance.obiRopeManager.cursors)
                {
                    item.ChangeLength(item.GetComponent<ObiRope>().restLength - .01f);
                }
                if (GameManager.Instance.obiRopeManager.obiRopes[0].restLength <= 0)
                {
                    //WIN
                    StartCoroutine(GameManager.Instance.WaitAndGameWin());
                    //Close rope, yarn and sticks
                    GameManager.Instance.obiRopeManager.CloseYarnThings();
                    m_animator.SetTrigger("Win");
                }
            }
        }

#elif UNITY_IOS || UNITY_ANDROID

        if (Input.touchCount > 0)
        {
            touch = Input.GetTouch(0);

            Vector3 move = Vector3.forward * zSpeed;

            if (touch.phase == TouchPhase.Moved)
            {
                move += Vector3.right * Input.GetAxis("Mouse X") * xSpeed;
            }
            controller.Move(move * speed * Time.deltaTime);

            if (GameManager.Instance.isScalingRope)
            {
                foreach (ObiRopeCursor item in GameManager.Instance.obiRopeManager.cursors)
                {
                    item.ChangeLength(item.GetComponent<ObiRope>().restLength - .01f);
                }
                if (GameManager.Instance.obiRopeManager.obiRopes[0].restLength <= 0)
                {
                    GameManager.Instance.isGameOver = true;
                    //WIN
                    StartCoroutine(GameManager.Instance.WaitAndGameWin());
                    //Close rope, yarn and sticks
                    GameManager.Instance.obiRopeManager.CloseYarnThings();
                    m_animator.SetTrigger("Win");
                }
            }
        }
#endif

        if (Input.GetMouseButtonDown(0))
        {
            if (GameManager.Instance.currentLevel == 1 && GameManager.Instance.Tuto1.activeSelf)
            {
                GameManager.Instance.Tuto1.SetActive(false);
            }
            if (GameManager.Instance.currentLevel == 1 && GameManager.Instance.Tuto2.activeSelf)
            {
                GameManager.Instance.Tuto2.SetActive(false);
            }
            m_animator.SetBool("Walking", true);
        }
        if (Input.GetMouseButtonUp(0))
        {
            m_animator.SetBool("Walking", false);
        }

        if (transform.position.y < -5)
        {
            //Lose
            StartCoroutine(GameManager.Instance.WaitAndGameLose());
            //Close rope, yarn and sticks
            GameManager.Instance.obiRopeManager.CloseYarnThings();
            m_animator.SetTrigger("Lose");
        }

        velocity.y += gravity * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime);

        GameManager.Instance.RoadSlider.value = (totalRoadLength - (FinishTransform.position.z - transform.position.z)) / totalRoadLength;
    }

    public GameObject collectParticle;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("NewYarn"))
        {
            GameObject go = Instantiate(collectParticle, other.transform.position, Quaternion.identity);
            go.GetComponent<ParticleSystem>().startColor = other.GetComponent<NewYarn>().color;
            Destroy(other.gameObject);
            GameManager.Instance.obiRopeManager.AddNewRope(other.GetComponent<NewYarn>().color);
            SoundManager.Instance.playSound(SoundManager.GameSounds.Collect);
        }
        else if (other.CompareTag("Line"))
        {
            other.GetComponent<Line>().OpenNet(this);
        }
        else if (other.CompareTag("Stitch"))
        {
            other.transform.GetChild(0).GetComponent<BoxCollider>().enabled = true;
            other.transform.GetChild(0).GetComponentInChildren<MeshRenderer>().material.color = GameManager.Instance.obiRopeManager.solver.colors[0];
            other.transform.GetChild(1).GetComponentInChildren<MeshRenderer>().material.color = GameManager.Instance.obiRopeManager.solver.colors[0];
            other.transform.GetChild(0).GetComponentInChildren<MeshRenderer>().material.SetFloat("_Fill", 1);
            other.transform.GetChild(1).GetComponentInChildren<MeshRenderer>().material.SetFloat("_Fill", 1);
            GameManager.Instance.playerControl.m_animator.SetBool("Knitting", true);
            foreach (var item in GameManager.Instance.obiRopeManager.cursors)
            {
                item.ChangeLength(item.GetComponent<ObiRope>().restLength - .05f);
            }
        }

        if (other.CompareTag("IsScaling"))
        {
            Debug.Log("asdsadf");
            if (GameManager.Instance.currentLevel == 1)
            {
                GameManager.Instance.Tuto2.SetActive(true);
            }
            GameManager.Instance.isScalingRope = true;
        }
        else if (other.CompareTag("Finish"))
        {
            GameManager.Instance.isGameOver = true;
            StartCoroutine(GameManager.Instance.WaitAndGameWin());
            m_animator.SetBool("Walking", false);
            //cam.GetComponent<CameraControl>().isCamTurn = true;
            //Close rope, yarn and sticks
            GameManager.Instance.obiRopeManager.CloseYarnThings();
            m_animator.SetTrigger("Win");
        }

        if (other.CompareTag("Finishcube"))
        {
            fc = other.GetComponentInChildren<FinishCube>();
        }
    }

    [HideInInspector]
    public FinishCube fc;
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Stitch"))
        {
            GameManager.Instance.playerControl.m_animator.SetBool("Knitting", false);
        }
    }
}
