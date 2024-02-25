using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrisonerScript10 : MonoBehaviour
{
    [SerializeField] private BoxCollider thisCollider;
    [SerializeField] public AIBaseScript10 agent;
    [SerializeField] public GameObject guardianHolder;
    public bool following = false;





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
    public void desGuardians()
    {
        guardianHolder.SetActive(false);
        Destroy(guardianHolder);
    }
    // Update is called once per frame
    public void Follow()
    {
        if (!agent.isActiveAndEnabled)
            agent.enabled = true;
    }
    public void Unfollow()
    {
        agent.enabled = false;
        following = false;
        thisCollider.enabled = true;
    }
}
