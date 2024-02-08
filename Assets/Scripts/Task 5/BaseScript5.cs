using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseScript5 : MonoBehaviour
{
    public void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Guardian")
        {
            other.gameObject.GetComponent<GuardianScript5>().BaseHit();
        }
    }
}
