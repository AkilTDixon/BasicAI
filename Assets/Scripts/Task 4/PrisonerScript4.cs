using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrisonerScript4 : MonoBehaviour
{
    [SerializeField] private BoxCollider thisCollider;
    [SerializeField] private AIBaseScript4 agent;

    private bool following = false;
    




    public void OnTriggerEnter(Collider other)
    {
        if (!following)
        {
            if (other.tag == "Hero")
            {
                thisCollider.enabled = false;
                following = true;
                agent.target = other.gameObject;
            }
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        agent.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (following)
        {
            if (!agent.isActiveAndEnabled)
                agent.enabled = true;

        }
    }
}
