using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    [SerializeField] float xSpeed, zSpeed;

    Vector3 translation;

    // Update is called once per frame
    //    void FixedUpdate()
    //    {
    //#if UNITY_EDITOR
    //        if (Input.GetMouseButton(0))
    //        {
    //            translation = new Vector3(Input.GetAxis("Mouse X") * xSpeed, 0, zSpeed) * Time.deltaTime;

    //            transform.Translate(translation, Space.World);
    //        }

    //#elif UNITY_IOS || UNITY_ANDROID

    //        if (Input.touchCount > 0)
    //        {
    //            touch = Input.GetTouch(0);
    //            if (touch.phase == TouchPhase.Moved)
    //            {
    //                transform.localPosition = new Vector3(Mathf.Clamp(transform.localPosition.x + touch.deltaPosition.x * 0.01f, -3, 3), transform.localPosition.y, transform.localPosition.z);
    //            }
    //            else if (touch.phase == TouchPhase.Began)
    //            {
    //                //save began touch 2d point
    //                firstPressPos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
    //            }
    //            transform.position += new Vector3(0, 0, Time.deltaTime * ZSpeed);
    //        }

    //#endif 

    //        // Don't go outside of the map
    //        Vector3 pos = transform.position;

    //        if (transform.position.x < -2f)
    //        {
    //            pos.x = -2f;
    //        }
    //        else if (transform.position.x > 2f)
    //        {
    //            pos.x = 2f;
    //        }

    //        transform.position = pos;

    //        CheckGround();
    //    }

    public CharacterController controller;

    public float speed = 12f;
    public float gravity = -9.81f;
    public float jumpHeight = 3f;

    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    Vector3 velocity;

    bool isGrounded;
    public Animator m_animator;

    private void Update()
    {
        if (!GameManager.Instance.isStarted || GameManager.Instance.isFinished)
        {
            return;
        }
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        if (Input.GetMouseButton(0))
        {
            Vector3 move = Vector3.right * Input.GetAxis("Mouse X") * xSpeed + Vector3.forward * zSpeed;

            controller.Move(move * speed * Time.deltaTime);
        }
        if (Input.GetMouseButtonDown(0))
        {
            m_animator.SetBool("Walking", true);
        }
        else if (Input.GetMouseButtonUp(0))
        {
            m_animator.SetBool("Walking", false);
        }

        velocity.y += gravity * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime);
    }

    public void CheckGround()
    {
        RaycastHit hit;
        // Does the ray intersect any objects excluding the player layer
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.down), out hit, 3f))
        {

        }
        else
        {
            Debug.Log("Did not Hit");
            GetComponent<Rigidbody>().isKinematic = false;
            GetComponent<Rigidbody>().useGravity = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "endGame")
        {
            Destroy(other.gameObject);
            //animator.SetBool("endGame", true);
            //GameManager.Instance.endGame = true;
            //ropeSpawn.Delete();
        }
        else if (other.CompareTag("NewYarn"))
        {
            Destroy(other.gameObject);
            GameManager.Instance.ropeManager.AddNewRope(other, other.GetComponent<NewYarn>().color);
        }
        else if (other.CompareTag("Line"))
        {
            other.GetComponent<Line>().playerControl = this;
            other.GetComponent<Line>().OpenNet();
        }
        else if (other.CompareTag("Finish"))
        {
            GameManager.Instance.isFinished = true;
            GameManager.Instance.TapToTryAgainButton.SetActive(true);
            m_animator.SetBool("Walking", false);
        }
    }
}
