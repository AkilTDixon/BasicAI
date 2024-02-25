using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIBaseScript9 : MonoBehaviour
{
    public enum AIType
    {
        Hero,
        Prisoner,
        Guardian
    };

    [SerializeField] AIType type;

    public LayerMask targetMask;
    public LayerMask blockedMask;

    public float maxSpeed;
    public GameObject target;
    public Vector3 targetPath;
    public Vector3 Velocity;
    public int pathCounter = 0;
    private Rigidbody body;

    // Start is called before the first frame update
    void Start()
    {
        body = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        body.velocity = Vector3.zero;
        body.angularVelocity = Vector3.zero;
        if (type == AIType.Hero || type == AIType.Prisoner)
        {


            float sp = maxSpeed;
            if (target)
            {
                targetPath = target.transform.position - transform.position;

                if (type == AIType.Hero)
                {


                    HeroScript9 hs = GetComponent<HeroScript9>();

                    if (!hs.victory)
                    {
                        if (hs.detected)
                        {
                            hs.avoidance = false;
                            GameObject closestGuardian = hs.GetClosestGuardian();
                            //change target to the guardian
                            //flee from the new target
                            if (hs.strat == HeroScript9.Strategy.Flee)
                            {
                                Flee(closestGuardian, sp);

                            }
                            else if (hs.strat == HeroScript9.Strategy.Destroy)
                            {

                                if (hs.HoldingObjects.Count > 0)
                                {
                                    Flee(closestGuardian, sp);

                                }
                                else if (closestGuardian)
                                {
                                    sp = closestGuardian.gameObject.GetComponent<AIBaseScript9>().maxSpeed;

                                    //I need to use the pathfinder to get a path back to the base

                                    Pathfind(hs);
                                    //targetPath = hs.GetBaseDirectionLine();

                                }
                            }
                            else if (hs.strat == HeroScript9.Strategy.Hunt)
                            {
                                if (hs.HoldingObjects.Count == 0)
                                {
                                    if (closestGuardian)
                                    {
                                        GuardianScript9 gs = closestGuardian.GetComponent<GuardianScript9>();

                                        float distToCapture = ((gs.transform.position - transform.position).magnitude - gs.captureRadius) * 0.45f;
                                        //distToCapture = Mathf.Clamp(distToCapture, 0.001f, 1f);


                                        sp = closestGuardian.gameObject.GetComponent<AIBaseScript9>().maxSpeed / distToCapture;

                                        if (sp > maxSpeed)
                                            sp = maxSpeed;
                                        //I need to use the pathfinder to get a path back to the base

                                        Pathfind(hs);
                                        //targetPath = hs.GetBaseDirectionLine();
                                    }
                                }
                                else
                                {
/*                                    if (closestGuardian)
                                    {
                                        Flee(closestGuardian, sp);
                                    }
                                    else*/
                                        Pathfind(hs);
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
                        else
                        {
                            Pathfind(hs);
                        }
                    }
                }

                Vector3 steeringForce;
                Quaternion rot;
                Velocity.y = 0;
                GetSteeringSum(out steeringForce, out rot);

                transform.rotation *= rot;
                if (transform.rotation.x != 0 || transform.rotation.z != 0)
                {
                    Quaternion q = new Quaternion();
                    q = transform.rotation;
                    q.x = 0;
                    q.z = 0;

                    transform.rotation = q;

                }


                Vector3 acceleration = steeringForce;
                Velocity += acceleration * Time.deltaTime;
                Velocity = Vector3.ClampMagnitude(Velocity, sp);

                Velocity.y = 0;

                transform.position += Velocity * Time.deltaTime;
            }

        }
        else if (type == AIType.Guardian)
        {
            GuardianScript9 gs = GetComponent<GuardianScript9>();
            Velocity.y = 0;
            targetPath = gs.GetTargetPath();


            Vector3 steeringForce;
            Quaternion rot;

            GetSteeringSum(out steeringForce, out rot);


            transform.rotation *= rot;

            if (transform.rotation.x != 0 || transform.rotation.z != 0)
            {
                Quaternion q = new Quaternion();
                q = transform.rotation;
                q.x = 0;
                q.z = 0;

                transform.rotation = q;

            }
            Vector3 acceleration = steeringForce;
            Velocity += acceleration * Time.deltaTime;
            Velocity = Vector3.ClampMagnitude(Velocity, maxSpeed);

            Velocity.y = 0;


            transform.position += Velocity * Time.deltaTime;

        }

    }

    public void Pathfind(GuardianScript9 gs)
    {
        if (gs.foundPath)
        {
            if (pathCounter < gs.pathPoints.Count - 1)
            {

                int index = gs.pathPoints.Count - 1;
                float mag1 = (transform.position - gs.pathPoints[index]).magnitude;
                float mag2 = (gs.pathPoints[pathCounter] - gs.pathPoints[index]).magnitude;


                if ((gs.pathPoints[pathCounter] - transform.position).magnitude <= 3f)
                {
                    RaycastHit hit1, hit2;



                    if (Physics.Raycast(transform.position, (gs.pathPoints[pathCounter] - transform.position), Mathf.Infinity, blockedMask))
                    {
                        if ((gs.pathPoints[pathCounter] - transform.position).magnitude <= 2f)
                            if (mag1 <= mag2)
                                pathCounter++;
                    }
                    else if (mag1 <= mag2)
                        pathCounter++;


                }
            }
            /*            else
                            if ((gs.pathPoints[pathCounter] - transform.position).magnitude <= 0.25f)
                                pathCounter++;*/

            if (pathCounter >= gs.pathPoints.Count)
                pathCounter = 0;
            if (gs.pathPoints.Count > 0)
                targetPath = gs.pathPoints[pathCounter] - transform.position;
            else
                targetPath = target.transform.position - transform.position;
        }
        else if (!gs.foundPath)
        {
            pathCounter = 0;
        }
    }
    private void Pathfind(HeroScript9 hs)
    {
        if (hs.foundPath)
        {
            RaycastHit hit;
            if (pathCounter < hs.pathPoints.Count - 1)
            {
                int index = hs.pathPoints.Count - 1;
                float mag1 = (transform.position - hs.pathPoints[index]).magnitude;
                float mag2 = (hs.pathPoints[pathCounter] - hs.pathPoints[index]).magnitude;



                if ((hs.pathPoints[pathCounter] - transform.position).magnitude <= 4f)
                {


                    if (Physics.SphereCast(transform.position, hs.pf.castWidth, (hs.pathPoints[pathCounter] - transform.position), out hit, Mathf.Infinity, blockedMask))
                    {
                        if ((hs.pathPoints[pathCounter] - transform.position).magnitude <= 2f)
                            if (mag1 <= mag2)
                                pathCounter++;
                    }
                    else if (mag1 <= mag2)
                        pathCounter++;


                }
            }

            /*            else
                            if ((hs.pathPoints[pathCounter] - transform.position).magnitude <= 0.25f)
                                pathCounter++;*/

            if (pathCounter >= hs.pathPoints.Count)
                pathCounter = 0;

            targetPath = hs.pathPoints[pathCounter] - transform.position;
        }
        else if (!hs.foundPath)
        {
            pathCounter = 0;
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
