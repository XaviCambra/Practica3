using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public Transform m_StartPoint;

    public Vector3 GetStartPointPosition()
    {
        return m_StartPoint.position;
    }
}
