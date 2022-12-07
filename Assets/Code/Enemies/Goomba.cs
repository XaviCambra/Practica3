using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Goomba : MonoBehaviour, IRestartGameElement
{
    Animator m_Animator;
    public float m_KillTime = 0.5f;
    public float m_KillScale = 0.2f;
    public float m_Alert = 2.0f;
    public Transform m_Mario;
    NavMeshPath m_AttackPath;
    public float m_Speed = 5.0f;
    CharacterController m_CharacterController;
    NavMeshAgent m_NavMeshAgent;
    
    public float m_EyesHeight;
    public float m_VisualConeAngle = 60.0f;
    public LayerMask m_SightLayerMask;
    public float m_SightDistance = 8.0f;
    public float m_EyesPlayerHeight;
    public float m_SafeDistance = 2.0f;

    float m_TimeOnAlert = 0.0f;

    public List<Transform> m_PatrolTargets;
    int m_CurrentPatrolTargetId = 0;

    Vector3 m_PlayerPosition;

    public enum TStates
    {
        PATROL,
        ALERT,
        ATTACK,
        DEAD
    }

    TStates m_CurrentState;

    private void Awake()
    {
        m_NavMeshAgent = GetComponent<NavMeshAgent>();
        m_Animator = GetComponent<Animator>();
    }

    private void Start()
    {
        m_CurrentState = TStates.PATROL;
        GameController.GetGameController().AddRestartGameElement(this);
        m_AttackPath = new NavMeshPath();
    }

    private void Update()
    {
        switch (m_CurrentState)
        {
            case TStates.PATROL:
                UpdatePatrolState();
                break;
            case TStates.ALERT:
                UpdateAlertState();
                break;
            case TStates.ATTACK:
                UpdateAttackState();
                break;
            case TStates.DEAD:
                UpdateDeadState();
                break;
        }
    }

    void SetPatrolState()
    {
        MoveToNextPatrolPosition();
        m_NavMeshAgent.isStopped = false;
        m_Animator.SetTrigger("Walk");
        m_CurrentState = TStates.PATROL;
    }

    void UpdatePatrolState()
    {
        if (SeesPlayer())
        {
            m_NavMeshAgent.isStopped = true;
            SetAlertState();
        }
        if (PatrolTargetPositionArrived())
        {
            MoveToNextPatrolPosition();
        }
    }
    void UpdateDeadState()
    {
        m_NavMeshAgent.isStopped = true;
    }

    void SetAlertState()
    {
        m_TimeOnAlert = 0;
        m_Animator.SetTrigger("Alert");
        m_CurrentState = TStates.ALERT;
    }

    void UpdateAlertState()
    {
        if(m_TimeOnAlert >= m_Alert)
        {
            m_CurrentState = TStates.ATTACK;
            return;
        }
        m_PlayerPosition = GameController.GetGameController().GetPlayer().transform.position;
        transform.LookAt(m_PlayerPosition);
        m_TimeOnAlert += Time.deltaTime;
    }
    void UpdateAttackState()
    {
        Vector3 l_playerPosition = GameController.GetGameController().GetPlayer().transform.position;
        transform.position = Vector3.MoveTowards(transform.position, m_PlayerPosition, m_NavMeshAgent.speed * Time.deltaTime);
        Debug.Log(Vector3.Distance(l_playerPosition, transform.position));  
        if(Vector3.Distance(l_playerPosition, transform.position) < 1)
        {
            GameController.GetGameController().GetPlayer().Hit();
            GameController.GetGameController().GetPlayer().JumpOutOfGoomba(transform.position);
        }
        if (transform.position == m_PlayerPosition)
        {
            m_NavMeshAgent.isStopped = true;
            SetPatrolState();
        }

    }

    public void Kill()
    {
        transform.localScale = new Vector3(1.0f, m_KillScale, 1.0f);
        m_CurrentState = TStates.DEAD;
        StartCoroutine(Hide());
    }
    IEnumerator Hide()
    {
        yield return new WaitForSeconds(m_KillTime);
        gameObject.SetActive(false);

    }

    public void RestartGame()
    {
        gameObject.SetActive(true);
    }

    bool PatrolTargetPositionArrived()
    {
        return !m_NavMeshAgent.hasPath && !m_NavMeshAgent.pathPending && m_NavMeshAgent.pathStatus == NavMeshPathStatus.PathComplete;
    }

    void MoveToNextPatrolPosition()
    {
        ++m_CurrentPatrolTargetId;
        if (m_CurrentPatrolTargetId >= m_PatrolTargets.Count)
            m_CurrentPatrolTargetId = 0;
        m_NavMeshAgent.destination = m_PatrolTargets[m_CurrentPatrolTargetId].position;
    } 
    bool SeesPlayer()
    {
        Vector3 l_playerPosition = GameController.GetGameController().GetPlayer().transform.position;
        Vector3 l_DirectionToPlayerXZ = l_playerPosition - transform.position;
        l_DirectionToPlayerXZ.y = 0.0f;
        l_DirectionToPlayerXZ.Normalize();
        Vector3 l_ForwardXZ = transform.forward;
        l_ForwardXZ.y = 0.0f;
        l_ForwardXZ.Normalize();

        if (Vector3.Distance(l_playerPosition, transform.position) > m_SightDistance)
        {
            return false;
        }

        Vector3 l_EyesPosition = transform.position + Vector3.up * m_EyesHeight;
        Vector3 l_PlayerEyesPosition = l_playerPosition + Vector3.up * m_EyesPlayerHeight;
        Vector3 l_Direction = l_PlayerEyesPosition - l_EyesPosition;

        float l_Lenght = l_Direction.magnitude;
        l_Direction /= l_Lenght;


        Ray l_Ray = new Ray(l_EyesPosition, l_Direction);

        
        return Vector3.Dot(l_ForwardXZ, l_DirectionToPlayerXZ) > Mathf.Cos(m_VisualConeAngle * Mathf.Deg2Rad / 2.0f) && !Physics.Raycast(l_Ray, l_Lenght, m_SightLayerMask.value);
    }
}

   
