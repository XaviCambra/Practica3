using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//mover a otro script
public interface IRestartGameElement
{
    void RestartGame();
}
public class MarioPlayerController : MonoBehaviour, IRestartGameElement
{
    public enum TPunchType
    {
        RIGHT_HAND = 0,
        LEFT_HAND,
        KICK
    }
    
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

    [Header("Punch")]
    public float m_ComboPunchTime = 2.5f;
    float m_ComboPunchCurrentTime;
    TPunchType m_CurrentComboPunch;
    public Collider m_LeftHandCollider;
    public Collider m_RightHandCollider;
    public Collider m_KickCollider;
    bool m_IsPunchEnabled = false;


    public float m_ElevatorDotAngle = 0.95f;
    Collider m_CurrentElevatorCollider = null;

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
        m_ComboPunchCurrentTime = -m_ComboPunchTime;
        m_LeftHandCollider.gameObject.SetActive(false);
        m_RightHandCollider.gameObject.SetActive(false);
        m_KickCollider.gameObject.SetActive(false);
        GameController.GetGameController().AddRestartGameElement(this);
        GameController.GetGameController().SetPlayer(this);
    }

    public void SetPunchActive(TPunchType PunchType, bool Active)
    {
        if (PunchType == TPunchType.RIGHT_HAND)
            m_RightHandCollider.gameObject.SetActive(Active);
        else if (PunchType == TPunchType.LEFT_HAND)
            m_LeftHandCollider.gameObject.SetActive(Active);
        else if (PunchType == TPunchType.KICK)
            m_KickCollider.gameObject.SetActive(Active);
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
        if (Input.GetMouseButtonDown(0) && CanPunch())
        {
            if (MustRestartComboPunch())
                SetComboPunch(TPunchType.RIGHT_HAND);
            else
                NextComboPunch();
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

    private void LateUpdate()
    {
        if(m_CurrentElevatorCollider!= null)
        {
            Vector3 l_EulerRotation = transform.rotation.eulerAngles;
            transform.rotation = Quaternion.Euler(0.0f, l_EulerRotation.y, 0.0f);
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
        if(other.tag == "Elevator" && CanAttachToElevator(other))
        {
            AttachToElevator(other);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if(other.tag == "Elevator" && other == m_CurrentElevatorCollider)
        {
            DetachElevator();
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if(other.tag == "Elevator")
        {
            if (m_CurrentElevatorCollider != null && Vector3.Dot(other.transform.up, Vector3.up) < m_ElevatorDotAngle)
                DetachElevator();
            if (CanAttachToElevator(other))
                AttachToElevator(other);
        }
    }

    bool CanAttachToElevator(Collider other)
    {
        return m_CurrentElevatorCollider == null && Vector3.Dot(other.transform.up, Vector3.up) >= m_ElevatorDotAngle;
    }

    void AttachToElevator(Collider other)
    {
        transform.SetParent(other.transform);
        m_CurrentElevatorCollider = other;
    }

    void DetachElevator()
    {
        transform.SetParent(null);
        m_CurrentElevatorCollider = null;
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
    bool CanPunch()
    {
        return !m_IsPunchEnabled;
    }

    public void SetIsPunchEnabled(bool IsPunchEnabled)
    {
        m_IsPunchEnabled = IsPunchEnabled;
    }
    bool MustRestartComboPunch()
    {
        return (Time.time - m_ComboPunchCurrentTime) > m_ComboPunchTime;
    }

    void SetComboPunch(TPunchType PunchType)
    {
        m_CurrentComboPunch = PunchType;
        m_ComboPunchCurrentTime = Time.time;
        m_IsPunchEnabled = true;
        if (m_CurrentComboPunch == TPunchType.RIGHT_HAND)
            m_Animator.SetTrigger("PunchRight and");
        else if (m_CurrentComboPunch == TPunchType.LEFT_HAND)
            m_Animator.SetTrigger("PunchLeftHand");
        else if (m_CurrentComboPunch == TPunchType.KICK)
            m_Animator.SetTrigger("PunchKick");
    }

    void NextComboPunch()
    {
        if (m_CurrentComboPunch == TPunchType.RIGHT_HAND)
            SetComboPunch(TPunchType.LEFT_HAND);
        else if (m_CurrentComboPunch == TPunchType.LEFT_HAND)
            SetComboPunch(TPunchType.KICK);
        else if (m_CurrentComboPunch == TPunchType.KICK)
            SetComboPunch(TPunchType.RIGHT_HAND);
    }
    
}
