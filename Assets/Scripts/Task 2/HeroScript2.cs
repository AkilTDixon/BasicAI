using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroScript2 : MonoBehaviour
{
    [SerializeField] private List<GameObject> PrisonerObjects;
    [SerializeField] public GameObject BasePoint;
    [SerializeField] public List<GameObject> HoldingObjects;
    [SerializeField] public AIBaseScript2 agent;
    [SerializeField] public float viewRadius;
    [SerializeField] public float viewAngle;

    public LayerMask targetMask;
    public LayerMask blockedMask;

    public GameObject seenGuardian;
    public List<GameObject> detectedBy;
    public bool detected = false;
    public bool victory = false;
    public bool avoidance = false;
    public GameObject closestGuardian;

    public Vector3 avoidOffset = Vector3.zero;

    public void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Prisoner")
        {

            agent.Velocity = Vector3.zero;
            HoldingObjects.Add(other.gameObject);
            agent.target = BasePoint;
        }
        if (other.tag == "Base")
        {
            //if hero went back to base to destroy a guardian, don't do this
            if (HoldingObjects.Count > 0)
            {
                agent.Velocity = Vector3.zero;
                for (int i = 0; i < HoldingObjects.Count; i++)
                {
                    PrisonerObjects.Remove(HoldingObjects[i]);
                    Destroy(HoldingObjects[i]);
                }
                HoldingObjects.Clear();


                GetClosestPrisoner();
            }
        }
    }

    public Vector3 GetBaseDirectionLine()
    {
        return (BasePoint.transform.position - transform.position).normalized;
    }

    // Start is called before the first frame update
    void Start()
    {
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

            dist = (transform.position - detectedBy[i].transform.position).magnitude;
            if (i == 0)
            {
                shortest = dist;
            }
            else if (dist < shortest)
            {
                dist = shortest;
                ret = detectedBy[i];

            }
        }
        return ret;
    }
    void GetClosestPrisoner()
    {
        float shortest = -1;

        if (PrisonerObjects.Count == 0)
        {
            agent.target = BasePoint;

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
        Collider[] rangeChecks = Physics.OverlapSphere(transform.position, viewRadius, targetMask);

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


                if (!Physics.Raycast(transform.position, direction, dist, blockedMask))
                {
                    //Sees a guardian

                    GuardianScript2 gs = visible[index].GetComponent<GuardianScript2>();

                    seenGuardian = visible[index].gameObject;

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


                    Vector3 vecToCenter = transform.position - gs.transform.position;
                    float magOfVector = vecToCenter.magnitude;

                    Vector3 circlePoint = gs.transform.position + vecToCenter / (magOfVector) * gs.radius;
                    Vector3 guardianVector = gs.transform.position - circlePoint;




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
                        if (d <= 4)
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
            }
        }
        else
        {
            //Previously found a guardian, but it is no longer in view distance
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
            detected = false;
    }
}
