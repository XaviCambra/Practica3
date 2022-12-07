using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HUD : MonoBehaviour
{
    Animator m_Animator;
    public float m_Timer;
    private void Start()
    {
        m_Animator = GetComponent<Animator>();
        m_Timer = 0f;
    }
    private void Update()
    {
        if ( m_Timer > 0f)
        {
            m_Timer -= Time.deltaTime;
        }
        m_Animator.SetBool("Enter", m_Timer > 0);
    }
    public void ActionDone()
    {
        m_Timer = 3f;
    }
}