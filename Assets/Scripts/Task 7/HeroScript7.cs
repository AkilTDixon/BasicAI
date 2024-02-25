using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroScript7 : MonoBehaviour
{
    public enum Strategy
    {
        Flee,
        Destroy,
        Hunt
    };
    [SerializeField] public Strategy strat;

    [SerializeField] private List<GameObject> PrisonerObjects;
    [SerializeField] public GameObject BasePoint;
    [SerializeField] public List<GameObject> HoldingObjects;
    [SerializeField] public AIBaseScript7 agent;
    [SerializeField] public float viewRadius;
    [SerializeField] public float viewAngle;



    public List<Vector3> pathPoints;
    public PathfinderScript7 pf;
    private Vector3 baseDirection;
    public List<GameObject> detectedBy;
    public bool detected = false;
    public bool victory = false;
    public bool wait = false;
    public bool avoidance = false;
    public bool foundPath = false;
    public GameObject closestGuardian;

    public GameObject seenGuardian;
    private Helper h;

    public Vector3 avoidOffset = Vector3.zero;

    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name == "Saw")
        {
            GameInfoScript.Instance.GameOver("Task 7");
        }
    }
    public void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Prisoner")
        {
            other.GetComponent<PrisonerScript7>().Follow();
            //agent.Velocity = Vector3.zero;
            HoldingObjects.Add(other.gameObject);
            agent.target = BasePoint;
            foundPath = false;
            pathPoints.Clear();
        }
        if (other.tag == "Base")
        {
            //if hero went back to base to destroy a guardian, don't do this
            if (HoldingObjects.Count > 0)
            {
                //agent.Velocity = Vector3.zero;
                for (int i = 0; i < HoldingObjects.Count; i++)
                {
                    PrisonerObjects.Remove(HoldingObjects[i]);
                    Destroy(HoldingObjects[i]);
                }
                HoldingObjects.Clear();

                foundPath = false;
                pathPoints.Clear();
                GetClosestPrisoner();
            }
        }
    }
    public void SetBaseDirectionLine()
    {
        baseDirection = (BasePoint.transform.position - transform.position).normalized;
    }
    public Vector3 GetBaseDirectionLine()
    {
        return baseDirection;
    }
    public void GetPathToBase()
    {
        agent.target = BasePoint;
        foundPath = false;
        pathPoints.Clear();
        agent.pathCounter = 0;
        foundPath = pf.Prelim(agent.target.transform.position, out pathPoints);
        pathPoints.Add(BasePoint.transform.position);

        Vector3 d = (pathPoints[pathPoints.Count - 1] - pathPoints[pathPoints.Count - 2]).normalized;

        pathPoints.Add(BasePoint.transform.position + d * 2f);
    }
    // Start is called before the first frame update
    void Start()
    {
        h = new Helper();
        pf = GetComponent<PathfinderScript7>();
        StartCoroutine(viewRoutine());
        GetClosestPrisoner();
    }

    private int GetClosestSeenGuardian(List<Transform> targets)
    {
        float dist, shortest;
        int ret = 0;
        shortest = (targets[0].position - transform.position).magnitude;

        for (int i = 0; i < targets.Count; i++)
        {
            dist = (targets[i].position - transform.position).magnitude;
            if (dist < shortest)
            {
                shortest = dist;
                ret = i;
            }
        }


        return ret;

    }
    public GameObject GetClosestGuardian()
    {
        if (detectedBy.Count == 0)
            return null;


        float dist, shortest;
        GameObject ret = detectedBy[0];
        shortest = -1;

        for (int i = 0; i < detectedBy.Count; i++)
        {
            if (detectedBy[i])
            {
                dist = (transform.position - detectedBy[i].transform.position).magnitude;
                if (i == 0)
                {
                    shortest = dist;
                }
                else if (dist < shortest)
                {
                    shortest = dist;
                    ret = detectedBy[i];

                }
            }
        }
        return ret;
    }
    public void GetClosestPrisoner()
    {
        float shortest = -1;

        if (PrisonerObjects.Count == 0)
        {
            GameInfoScript.Instance.GameOver("Task 7");
            agent.target = BasePoint;
            foundPath = true;
            Destroy(gameObject);

            return;
        }

        for (int i = 0; i < PrisonerObjects.Count; i++)
        {
            float dist = (transform.position - PrisonerObjects[i].transform.position).magnitude;
            if (shortest == -1)
            {
                shortest = dist;
                agent.target = PrisonerObjects[i];
            }
            else if (dist < shortest)
            {
                shortest = dist;
                agent.target = PrisonerObjects[i];
            }
        }
    }


    private void viewCheck()
    {
        RaycastHit ht;
        //if a wall suddenly appears between the current position and the path point
        if (pathPoints.Count > 0)
        {
            if (Physics.SphereCast(transform.position, pf.castWidth, (pathPoints[agent.pathCounter] - transform.position), out ht, (pathPoints[agent.pathCounter] - transform.position).magnitude, agent.blockedMask))
            {
                if (detected)
                {
                    GetPathToBase();
                }
                else
                {
                    foundPath = false;
                    //calculate a new path

                    pathPoints.Clear();
                    agent.pathCounter = 0;
                }
            }
        }


        if (!foundPath)
        {
            //foundPath = pf.StartPath(agent.target.transform.position, out pathPoints);
            foundPath = pf.Prelim(agent.target.transform.position, out pathPoints);
        }


        Collider[] rangeChecks = Physics.OverlapSphere(transform.position, viewRadius, agent.targetMask);

        if (rangeChecks.Length != 0)
        {

            //All guardian detections within the sphere

            List<Transform> targets = new List<Transform>();
            for (int i = 0; i < rangeChecks.Length; i++)
                targets.Add(rangeChecks[i].transform);


            //i need to determine the closest guardian to be worried about




            //list of indexes of which targets are inside the viewing angle
            List<Transform> visible = new List<Transform>();
            for (int i = 0; i < targets.Count; i++)
            {
                Vector3 d = ((targets[i].position - transform.position).normalized);

                if (Vector3.Angle(transform.forward, d) < viewAngle / 2f)
                    visible.Add(targets[i]);
            }
            int index = -1;




            if (visible.Count > 0)
            {
                index = GetClosestSeenGuardian(visible);
                float dist = Vector3.Distance(transform.position, visible[index].position);

                Vector3 direction = ((visible[index].position - transform.position).normalized);


                if (!Physics.Raycast(transform.position, direction, dist, agent.blockedMask))
                {
                    //Sees a guardian

                    GuardianScript7 gs = visible[index].GetComponent<GuardianScript7>();

                    seenGuardian = visible[index].gameObject;

                    Vector3 vecToCenter = transform.position - gs.transform.position;
                    float magOfVector = vecToCenter.magnitude;
                    Vector3 circlePoint = gs.transform.position + vecToCenter / (magOfVector) * gs.radius;
                    if (rangeChecks.Length > 2 || strat == Strategy.Hunt)
                    {
                        strat = Strategy.Hunt;


                        if ((transform.position - (circlePoint)).magnitude > 1f)
                        {
                            Debug.DrawLine(transform.position, seenGuardian.transform.position, Color.red);
                            agent.target = seenGuardian;
                            wait = false;
                        }
                        else
                        {
                            wait = true;
                        }

                    }
                    else
                    {
                        if (dist <= gs.captureRadius + 3)
                        {
                            wait = true;
                        }
                        else
                            wait = false;
                        /*
                         Given the vector, I need its equation


                        gs.transform.position + gs.viewLine1 * (t)

                        x = (gs.transform.position.x + gs.viewLine1.x * (t))
                        y = gs.transform.position.y + gs.viewLine1.y * (t)
                        z = gs.transform.position.z + gs.viewLine1.z * (t)

                        given the point transform.position, I want the shortest distance between this point and the vector

                        this will be perpendicular, meaning I need the dot product to be 0

                        x = (gs.transform.position.x + (gs.viewLine1.x * (t))) - transform.position.x
                        y = (gs.transform.position.y + (gs.viewLine1.y * (t))) - transform.position.y
                        z = (gs.transform.position.z + (gs.viewLine1.z * (t))) - transform.position.z


                        originX = (gs.transform.position.x - transform.position.x);
                        originY = (gs.transform.position.y - transform.position.y);
                        originZ = (gs.transform.position.z - transform.position.z);



                        Now for the dot product that must be 0
                        0 =  (gs.viewLine.x * originX) + (gs.viewLine.x)*(gs.viewLine1.x * t) + 
                             (gs.viewLine.y * originY) + (gs.viewLine.y)*(gs.viewLine1.y * t) + 
                             (gs.viewLine.z * originZ) + (gs.viewLine.z)*(gs.viewLine1.z * t)

                        -(gs.viewLine.x * originX) - (gs.viewLine.y * originY) - (gs.viewLine.z * originZ)
                        = 
                        (gs.viewLine.x)*(gs.viewLine1.x * t) + (gs.viewLine.y)*(gs.viewLine1.y * t) + (gs.viewLine.z)*(gs.viewLine1.z * t)

                        -(gs.viewLine.x * originX) - (gs.viewLine.y * originY) - (gs.viewLine.z * originZ)
                        = 
                        t*(gs.viewLine.x*gs.viewLine1.x) + (gs.viewLine.y*gs.viewLine1.y) + (gs.viewLine.z*gs.viewLine1.z)


                        -(gs.viewLine.x * originX) - (gs.viewLine.y * originY) - (gs.viewLine.z * originZ)
                        /
                        (gs.viewLine.x*gs.viewLine1.x) + (gs.viewLine.y*gs.viewLine1.y) + (gs.viewLine.z*gs.viewLine1.z)
                        = 
                        t




                        Now, I want a minimum and maximum t value

                        minimum value of t will be 0

                        maximum value of t is whichever t that gives me the extentP1

                        gs.extentP1.x = gs.transform.position.x + gs.viewLine1.x * t
                        gs.extentP1.y = gs.transform.position.y + gs.viewLine1.y * t
                        gs.extentP1.z = gs.transform.position.z + gs.viewLine1.z * t



                        maxT1 = (gs.extentP1 - gs.transform.position) / gs.viewLine1 

                         */


                        float originX, originY, originZ;
                        originX = (gs.transform.position.x - transform.position.x);
                        originY = (gs.transform.position.y - transform.position.y);
                        originZ = (gs.transform.position.z - transform.position.z);

                        float t1 = -(gs.viewLine1.x * originX) - (gs.viewLine1.y * originY) - (gs.viewLine1.z * originZ)
                        /
                        (gs.viewLine1.x * gs.viewLine1.x) + (gs.viewLine1.y * gs.viewLine1.y) + (gs.viewLine1.z * gs.viewLine1.z);


                        float t2 = -(gs.viewLine2.x * originX) - (gs.viewLine2.y * originY) - (gs.viewLine2.z * originZ)
                        /
                        (gs.viewLine2.x * gs.viewLine2.x) + (gs.viewLine2.y * gs.viewLine2.y) + (gs.viewLine2.z * gs.viewLine2.z);






                        Vector3 intersectionP1 = gs.transform.position + gs.viewLine1 * t1;
                        Vector3 intersectionP2 = gs.transform.position + gs.viewLine2 * t2;

                        float dist1 = (transform.position - intersectionP1).magnitude;
                        float dist2 = (transform.position - intersectionP2).magnitude;


                        /*
                         Equation of a circle
                         ( x - a )^2 + ( y - b )^2 + ( z - c )^2 = r^2

                        (a, b, c) is the center point

                        I have the center point and the radius

                        the shortest distance between a point and a circle will be

                        Vector

                         */



                        Vector3 guardianVector = gs.transform.position - circlePoint;



                        /*                    var theta1 = Mathf.Atan2(gs.viewLine1.x, gs.viewLine1.z);
                                            var thetaT = Mathf.Atan2(guardianVector.x, guardianVector.z);
                                            var theta2 = Mathf.Atan2(gs.viewLine2.x, gs.viewLine2.z);




                                            theta1 = theta1 * Mathf.Rad2Deg;
                                            thetaT = thetaT * Mathf.Rad2Deg;
                                            theta2 = theta2 * Mathf.Rad2Deg;

                                            if (theta1 < 0)
                                                theta1 += 360;
                                            if (theta2 < 0)
                                                theta2 += 360;
                                            if (thetaT < 0)
                                                thetaT += 360;*/






                        Debug.DrawLine(transform.position, circlePoint, Color.magenta);

                        if (dist1 < dist2)
                        {
                            //only worry about viewline1
                            if (checkAvoid(gs.maxT1, t1))
                            {

                                if (dist1 <= 5)
                                {
                                    avoidance = true;
                                    Debug.DrawRay(transform.position, -(transform.position - intersectionP1), Color.red);
                                    Vector3 dir = (transform.position - intersectionP1).normalized;
                                    avoidOffset = dir;
                                    StartCoroutine(avoidState());
                                }

                            }

                        }
                        else
                        {
                            //worry about viewline2
                            if (checkAvoid(gs.maxT2, t2))
                            {

                                if (dist2 <= 5)
                                {
                                    avoidance = true;
                                    Debug.DrawRay(transform.position, -(transform.position - intersectionP2), Color.red);
                                    Vector3 dir = (transform.position - intersectionP2).normalized;
                                    avoidOffset = dir;
                                    StartCoroutine(avoidState());

                                }
                            }


                        }

                        //Determining if the hero is in danger of reaching the guardian viewing arc
                        if (Vector3.Dot(Vector3.Cross(gs.viewLine1, guardianVector), Vector3.Cross(gs.viewLine1, gs.viewLine2)) >= 0
                            &&
                            Vector3.Dot(Vector3.Cross(gs.viewLine2, guardianVector), Vector3.Cross(gs.viewLine2, gs.viewLine1)) >= 0)
                        {
                            float d = (transform.position - circlePoint).magnitude;
                            float magCheck = 4;

                            if (strat == Strategy.Hunt)
                                magCheck = 1;

                            if (d <= magCheck)
                            {
                                if (!avoidance)
                                    StartCoroutine(avoidState());
                                Debug.DrawLine(gs.transform.position, circlePoint, Color.cyan);
                                avoidance = true;
                                Vector3 dir = (transform.position - circlePoint).normalized;
                                avoidOffset = dir;

                            }
                        }
                    }
                }
                else
                {

                    //A guardian is there, but blocked by a wall
                    avoidance = false;
                    avoidOffset = Vector3.zero;
                }
            }
            else
            {

                //No longer in the view radius

                wait = false;
            }
        }
        else
        {
            //if (foundPlayer)
            //Previously found a guardian, but it is no longer in view distance
            wait = false;
            avoidance = false;
            avoidOffset = Vector3.zero;
        }

    }

    private bool checkAvoid(float max, float t)
    {
        if (max > 0)
        {
            if (t >= 0 && t < max)
                return true;

        }
        else if (max < 0)
        {
            if (t <= 0 && t > max)
                return true;
        }


        return false;
    }
    private bool onCircleArc(Vector3 p, Vector3 c1, Vector3 c2)
    {


        return false;
    }
    private IEnumerator viewRoutine()
    {

        while (true)
        {

            yield return new WaitForSeconds(0.2f);
            viewCheck();
        }
    }

    public IEnumerator avoidState()
    {
        yield return new WaitForSeconds(1);
        avoidance = false;
        avoidOffset = Vector3.zero;
    }

    public IEnumerator fleeState(GameObject g)
    {



        yield return new WaitForSeconds(2);
        detectedBy.Remove(g);
        if (detectedBy.Count == 0)
        {
            detected = false;
            foundPath = false;
            pathPoints.Clear();
            GetClosestPrisoner();
        }
    }

}
