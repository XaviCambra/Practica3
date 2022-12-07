using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Punch : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Goomba")
        {
            other.GetComponent<Goomba>().Kill();
        }
    }
}
