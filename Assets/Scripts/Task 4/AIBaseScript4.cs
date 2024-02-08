using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIBaseScript4 : MonoBehaviour
{
    public enum AIType
    {
        Hero,
        Prisoner,
        Guardian
    };
    public enum Strategy
    {
        Flee,
        Destroy
    };
    [SerializeField] AIType type;
    [SerializeField] Strategy strat;


    public float maxSpeed;
    public GameObject target;
    public Vector3 targetPath;
    public Vector3 Velocity;



    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (type == AIType.Hero || type == AIType.Prisoner)
        {
            float sp = maxSpeed;
            if (target)
            {
                targetPath = target.transform.position - transform.position;

                if (type == AIType.Hero)
                {
                    
                    HeroScript4 hs = GetComponent<HeroScript4>();

                    if (!hs.victory)
                    {
                        if (hs.detected)
                        {
                            hs.avoidance = false;
                            GameObject closestGuardian = hs.GetClosestGuardian();
                            //change target to the guardian
                            //flee from the new target
                            if (strat == Strategy.Flee)
                            {
                                Flee(closestGuardian, sp);

                            }
                            else if (strat == Strategy.Destroy)
                            {
                                
                                if (hs.HoldingObjects.Count > 0)
                                {
                                    Flee(closestGuardian, sp);
                                
                                }
                                else if (closestGuardian)
                                {
                                    sp = closestGuardian.gameObject.GetComponent<AIBaseScript4>().maxSpeed;
                                    targetPath = hs.GetBaseDirectionLine();

                                }
                            }


                        }
                        else if (hs.avoidance)
                        {
                            Velocity += ((hs.avoidOffset));
                        }
                        else if (hs.wait)
                        {
                            if (hs.seenGuardian)
                                targetPath = -Velocity;
                        }
                    }
                }

                Vector3 steeringForce;
                Quaternion rot;
                Velocity.y = 0;
                GetSteeringSum(out steeringForce, out rot);

                transform.rotation *= rot;
                Vector3 acceleration = steeringForce;
                Velocity += acceleration * Time.deltaTime;
                Velocity = Vector3.ClampMagnitude(Velocity, sp);

                Velocity.y = 0;

                transform.position += Velocity * Time.deltaTime;
            }

        }
        else if (type == AIType.Guardian)
        {
            GuardianScript4 gs = GetComponent<GuardianScript4>();
            Velocity.y = 0;
            targetPath = gs.GetTargetPath();


                Vector3 steeringForce;
                Quaternion rot;

                GetSteeringSum(out steeringForce, out rot);


                transform.rotation *= rot;
          
            Vector3 acceleration = steeringForce;
                Velocity += acceleration * Time.deltaTime;
                Velocity = Vector3.ClampMagnitude(Velocity, maxSpeed);

            Velocity.y = 0;


            transform.position += Velocity * Time.deltaTime;

        }
        
    }
    private void Flee(GameObject from, float sp)
    {
        Vector3 guardianPosition = from.transform.position;
        guardianPosition.y = transform.position.y;
        targetPath = transform.position - guardianPosition;
        targetPath = targetPath.normalized * sp;
    }
    private Quaternion FaceTarget()
    {
        Vector3 tpos = target.transform.position;
        tpos.y = transform.position.y;


        Vector3 direction = tpos - transform.position;

        Quaternion rot;


        if ((direction.normalized == transform.forward || Mathf.Approximately(direction.magnitude, 0)))
            rot = transform.rotation;
        else
            rot = Quaternion.LookRotation(direction);

        return Quaternion.FromToRotation(transform.forward, rot * Vector3.forward);
    }
    private Quaternion LookWhereYouAreGoing()
    {
        if (Velocity == Vector3.zero)
            return Quaternion.FromToRotation(transform.forward, transform.rotation * Vector3.forward);

        return Quaternion.FromToRotation(transform.forward, Quaternion.LookRotation(Velocity) * Vector3.forward);
    }



    public void GetSteeringSum(out Vector3 steeringForceSum, out Quaternion rotation)
    {
        steeringForceSum = Vector3.zero;
        rotation = Quaternion.identity;

        steeringForceSum += targetPath;
        
        if (type == AIType.Prisoner)
            rotation *= FaceTarget();
        else if (type == AIType.Hero || type == AIType.Guardian)
            rotation *= LookWhereYouAreGoing();

    }
}
