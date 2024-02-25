using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseScript7 : MonoBehaviour
{
    public void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Guardian")
        {
            other.gameObject.GetComponent<GuardianScript7>().BaseHit();
        }
    }
}
