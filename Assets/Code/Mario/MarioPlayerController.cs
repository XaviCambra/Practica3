using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarioPlayerController : MonoBehaviour
{
    Animator m_Animator;
    CharacterController m_CharacterController;
    public CameraController m_Camera;
    public float m_LerpRotationPct = 0.5f;
    public float m_WalkSpeed = 2.5f;
    public float m_RunSpeed= 6.5f;
    public float m_Life = 8.0f;
    public float m_Coins = 0.0f;

    Vector3 m_StartPosition;
    Quaternion m_StartRotation;

    //public Transform m_CheckPoint;
    public float m_VerticalSpeed = 0.0f;
    public float m_JumpSpeed = 10.0f;
    bool m_OnGround = false;

    private void Awake()
    {
        m_Animator = GetComponent<Animator>();
        m_CharacterController = GetComponent<CharacterController>();
    }

    void Start()
    {
        m_StartPosition = transform.position;
        m_StartRotation = transform.rotation;
        Debug.Log(m_StartPosition);
    }

    void Update()
    {
        float l_Speed = 0.0f;

        Vector3 l_ForwardCamera = m_Camera.transform.forward;
        Vector3 l_RightCamera = m_Camera.transform.right;
        l_ForwardCamera.y = 0.0f;
        l_RightCamera.y = 0.0f;
        l_ForwardCamera.Normalize();
        l_RightCamera.Normalize();
        bool l_HasMovement = false;

        Vector3 l_Movement = Vector3.zero;
        if (Input.GetKey(KeyCode.W))
        {
            l_HasMovement = true;
            l_Movement = l_ForwardCamera;
        }
        if (Input.GetKey(KeyCode.S))
        {
            l_HasMovement = true;
            l_Movement = -l_ForwardCamera;
        }
        if (Input.GetKey(KeyCode.A))
        {
            l_HasMovement = true;
            l_Movement -= l_RightCamera;
        }
        if (Input.GetKey(KeyCode.D))
        {
            l_HasMovement = true;
            l_Movement += l_RightCamera;
        }
        l_Movement.Normalize();

        float l_MovementSpeed = 0.0f;

        if (l_HasMovement)
        {
            Quaternion l_LookRotation = Quaternion.LookRotation(l_Movement);
            transform.rotation = Quaternion.Lerp(transform.rotation, l_LookRotation, m_LerpRotationPct);

            l_Speed = 0.5f;
            l_MovementSpeed = m_WalkSpeed;

            if (Input.GetKey(KeyCode.LeftShift))
            {
                l_Speed = 1.0f;
                l_MovementSpeed = m_RunSpeed;
            }
        }

        l_Movement = l_Movement * l_MovementSpeed * Time.deltaTime;

        m_Animator.SetFloat("Speed", l_Speed);
        if (Input.GetKeyDown(KeyCode.Space))
        {
            m_Animator.SetTrigger("Punch");
        }
        m_CharacterController.Move(l_Movement);

        m_VerticalSpeed = m_VerticalSpeed + Physics.gravity.y * Time.deltaTime;
        l_Movement.y = m_VerticalSpeed * Time.deltaTime;

        CollisionFlags l_CollisionFlags = m_CharacterController.Move(l_Movement);

        if ((l_CollisionFlags & CollisionFlags.Above) != 0 && m_VerticalSpeed > 0.0f)
        {
            m_VerticalSpeed = 0.0f;
        }
        if ((l_CollisionFlags & CollisionFlags.Below) != 0)
        {
            m_VerticalSpeed = 0.0f;
            m_OnGround = true;
        }
        else
        {
            m_OnGround = false;
        }

        if(m_Life == 0)
        {
            RestartGame();
        }
    }
    public float GetLife()
    {
        return m_Life;
    }
    public void AddLife()
    {
        m_Life++;
    }

    public void AddCoin()
    {
        m_Coins++;
        Debug.Log(m_Coins);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "healthItem")
        {
            other.GetComponent<Item>().Pick(this);
        }
        else if(other.tag == "coinItem")
        {
            other.GetComponent<Item>().Pick(this);
        }
        else if(other.tag == "checkpoint")
        {
            m_StartPosition = other.GetComponent<Checkpoint>().GetStartPointPosition();
            Debug.Log(m_StartPosition);
        }
        else if(other.tag == "deadZone")
        {
            Kill();
        }
    }
    void Kill()
    {
        m_Life = 0.0f;
        RestartGame();
    }
    public void RestartGame()
    {
        m_Life = 8.0f;
        m_CharacterController.enabled = false;
        transform.position = m_StartPosition;
        transform.rotation = m_StartRotation;
        m_CharacterController.enabled = true;
    }

    void Hit()
    {
        m_Life--;
        if(m_Life <= 0)
        {
            RestartGame();
        }
    }
}
