using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    [SerializeField] public float maxSpeed = 3f;
    [SerializeField] public Transform respawnPoint;
    [SerializeField] public float deathTime = 5f;
    [SerializeField] public LayerMask targetMask;
    [SerializeField] public LayerMask blockedMask;
    [SerializeField] public float captureRadius = 2f;

    public Vector3 Velocity = Vector3.zero;
    private Rigidbody body;
    public bool dead = false;
    private float theTime = 0f;
    private float captureCheckInterval = 0.2f;

    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name == "Saw")
        {
            PlayerDeath();
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        theTime = Time.time;
        body = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time > theTime + captureCheckInterval)
        {
            captureCheck();
            theTime = Time.time;
        }
        if (!dead)
        {
            float horizontal = Input.GetAxis("Vertical");
            float vertical = Input.GetAxis("Horizontal");

            Vector3 movement = new Vector3(vertical, 0, horizontal);
            movement = Vector3.ProjectOnPlane(movement, Vector3.up);
            body.velocity = Vector3.zero;
            body.angularVelocity = Vector3.zero;
            if (movement.magnitude != 0)
            {
                transform.rotation = Quaternion.LookRotation(movement);
                Velocity += movement;
                Velocity = Vector3.ClampMagnitude(Velocity, maxSpeed);
                Velocity.y = 0;

                transform.position += Velocity * Time.deltaTime;
            }
        }
    }

    private void captureCheck()
    {


        Collider[] rangeChecks = Physics.OverlapSphere(transform.position, captureRadius, targetMask);


        if (rangeChecks.Length != 0)
        {
            Transform target = rangeChecks[0].transform;
            Vector3 direction = (target.position - transform.position).normalized;

            for (int i = 0; i < rangeChecks.Length; i++)
            {
                target = rangeChecks[i].transform;
                direction = (target.position - transform.position).normalized;
                float dist = Vector3.Distance(transform.position, target.position);
                //Player captured
                if (!Physics.Raycast(transform.position, direction, dist, blockedMask))
                {
                    HeroScript10 hs = rangeChecks[i].GetComponent<HeroScript10>();

                    
                    if (hs.HoldingObjects.Count > 0)
                    {
                        for (int j = 0; j < hs.HoldingObjects.Count; j++)
                        {
                            PrisonerScript10 p = hs.HoldingObjects[j].GetComponent<PrisonerScript10>();
                            p.agent.target = gameObject;
                            GameInfoScript.Instance.guardianHolders.Remove(p.guardianHolder);
                            p.desGuardians();
                            hs.HoldingObjects.Remove(hs.HoldingObjects[j]);
                            GameInfoScript.Instance.PrisonerTaken();
                        }
                    }
                    GameInfoScript.Instance.HeroCapturedByPlayer(rangeChecks[i].gameObject);
                }

            }




        }
    }


    public void PlayerDeath()
    {
        body.velocity = Vector3.zero;
        body.angularVelocity = Vector3.zero;
        transform.position = respawnPoint.position;
        dead = true;
        StartCoroutine(deathWait());

    }
    public IEnumerator deathWait()
    {
        yield return new WaitForSeconds(deathTime);
        dead = false;
    }
}
