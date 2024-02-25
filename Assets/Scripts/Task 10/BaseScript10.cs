using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseScript10 : MonoBehaviour
{
    public void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Guardian")
        {
            other.gameObject.GetComponent<GuardianScript10>().BaseHit();
        }
        else if (other.tag == "Player")
        {
            other.gameObject.GetComponent<PlayerScript>().PlayerDeath();
        }
    }
}
