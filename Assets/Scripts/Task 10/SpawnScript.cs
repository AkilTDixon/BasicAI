using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnScript : MonoBehaviour
{

    [SerializeField] public Transform prisonerTransform;

    private Transform original;

    void Start()
    {
        original = prisonerTransform;
    }
    public void SpawnGuardian(GameObject guardian)
    {
        GameObject obj = Instantiate(guardian, original.position, transform.rotation, transform);
        GuardianScript10 gs = obj.GetComponent<GuardianScript10>();

        gs.ProtectPosition = prisonerTransform;
        gs.wander = true;
        
    }
}
