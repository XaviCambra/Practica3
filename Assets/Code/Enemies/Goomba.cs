using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Goomba : MonoBehaviour, IRestartGameElement
{
    public float m_KillTime = 0.5f;
    public float m_KillScale = 0.2f;
    public float m_Alert = 2.0f;
    public Transform m_Mario;
    NavMeshPath m_AttackPath;
    public float m_Speed = 5.0f;
    CharacterController m_CharacterController;

    public enum TStates
    {
        PATROL,
        ALERT,
        ATTACK,
        DEAD
    }

    TStates m_CurrentState;

    private void Start()
    {
        m_CurrentState = TStates.PATROL;
        GameController.GetGameController().AddRestartGameElement(this);
        m_AttackPath = new NavMeshPath();
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.J))
            m_CurrentState = TStates.ALERT;
        if(m_CurrentState == TStates.ALERT)
        {
            StartCoroutine(Surprise());
        }
        else if (m_CurrentState == TStates.ATTACK)
        {
            
        }
    }
    public void Kill()
    {
        transform.localScale = new Vector3(1.0f, m_KillScale, 1.0f);
        StartCoroutine(Hide());
    }
    IEnumerator Hide()
    {
        yield return new WaitForSeconds(m_KillTime);
        gameObject.SetActive(false);

    }
    IEnumerator Surprise()
    {
        yield return new WaitForSeconds(m_Alert);
        NavMesh.CalculatePath(transform.position, m_Mario.position, NavMesh.AllAreas, m_AttackPath);
 
        for(int i = 0; i< m_AttackPath.corners.Length-1;i++)
            Debug.DrawLine(m_AttackPath.corners[i], m_AttackPath.corners[i + 1], Color.red);
        m_CurrentState = TStates.ATTACK;
    }
    public void RestartGame()
    {
        gameObject.SetActive(true);
    }
}
