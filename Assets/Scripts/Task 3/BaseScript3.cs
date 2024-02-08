using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseScript3 : MonoBehaviour
{
    public void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Guardian")
        {
            other.gameObject.GetComponent<GuardianScript3>().BaseHit();
        }
    }
}
