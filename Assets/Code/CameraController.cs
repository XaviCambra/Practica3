using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform m_LookAtTransform;
    float m_Yaw = 0.0f;
    float m_Pitch = 0.0f;
    public float m_Distance = 5.0f;
    public float m_YawRotationalSpeed = 380.0f;
    public float m_PitchRotationalSpeed = 180.0f;

    public float m_MinPitch = 30.0f;
    public float m_MaxPitch = 60.0f;

    [Header("Debug")]
    public KeyCode m_DebugLockAngleKeyCode = KeyCode.I;
    public KeyCode m_DebugLockKeyCode = KeyCode.O;
    bool m_AngleLocked = false;
    bool m_AimLocked = true;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        m_AimLocked = Cursor.lockState == CursorLockMode.Locked;
    }

    #if UNITY_EDITOR
    void UpdateInputDebug()
    {
        if (Input.GetKeyDown(m_DebugLockAngleKeyCode))
            m_AngleLocked = !m_AngleLocked;
        if (Input.GetKeyDown(m_DebugLockKeyCode))
        {
            if (Cursor.lockState == CursorLockMode.Locked)
                Cursor.lockState = CursorLockMode.None;
            else
                Cursor.lockState = CursorLockMode.Locked;
            m_AimLocked = Cursor.lockState == CursorLockMode.Locked;
        }

    }
    #endif

    private void LateUpdate()
    {
        #if UNITY_EDITOR
                UpdateInputDebug();
        #endif

        float l_MouseX = Input.GetAxis("Mouse X");
        float l_MouseY = Input.GetAxis("Mouse Y");

        if (m_AngleLocked)
        {
            l_MouseX = 0.0f;
            l_MouseY = 0.0f;
        }
        
        
        m_Yaw += l_MouseX*m_YawRotationalSpeed*Time.deltaTime;
        m_Pitch += l_MouseY * m_PitchRotationalSpeed * Time.deltaTime;
        m_Pitch = Mathf.Clamp(m_Pitch, m_MinPitch, m_MaxPitch);

        Vector3 l_ForwardCamera = new Vector3(Mathf.Sin(m_Yaw * Mathf.Deg2Rad) * Mathf.Cos(m_Pitch * Mathf.Deg2Rad), Mathf.Sin(m_Pitch * Mathf.Deg2Rad), Mathf.Cos(m_Yaw * Mathf.Deg2Rad) * Mathf.Cos(m_Pitch * Mathf.Deg2Rad)));
        transform.position = m_LookAtTransform.position - l_ForwardCamera * m_Distance;
        transform.LookAt(m_LookAtTransform.position);
    }
}
