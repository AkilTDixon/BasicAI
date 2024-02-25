using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;

public class GuardianScript10 : MonoBehaviour
{
    [SerializeField] public GameObject HeroObject;
    [SerializeField] private float SearchDelay = 0.2f;
    [SerializeField] public Transform ProtectPosition;
    [SerializeField] public AIBaseScript10 agent;

    private Helper h = new Helper();
    public float angle, radius;

    public List<Vector3> pathPoints;
    public PathfinderScript10 pf;

    private bool foundPlayer;

    public float wanderDegreesDelta = 45;
    [Min(0)] public float wanderInterval = 0.75f;
    protected float wanderTimer = 0;

    private Vector3 lastWanderDirection;
    private Vector3 lastPath;
    private Vector3 avoidPath;

    private Vector3 targetPath;
    public bool wander = false;
    public bool avoiding = false;
    public bool foundPath = false;
    private HeroScript10 hs;
    public float captureRadius;
    private float maxSpeed;


    public Vector3 viewLine1;
    public Vector3 viewLine2;
    public Vector3 extentP1;
    public Vector3 extentP2;
    public float angle1, angle2;

    public float maxT1, maxT2, minT1, minT2;




    void Start()
    {
        pf = GetComponent<PathfinderScript10>();
        maxSpeed = GetComponent<AIBaseScript10>().maxSpeed;
        captureRadius = radius / 4f;
        StartCoroutine(viewRoutine());
    }

    void Update()
    {

        if (!foundPath)
        {
            if (wander)
            {
                wanderTimer += Time.deltaTime;


                // TODO: calculate linear component

                if (lastWanderDirection == Vector3.zero)
                    lastWanderDirection = transform.forward.normalized * maxSpeed;

                if (lastPath == Vector3.zero)
                    lastPath = transform.forward;

                targetPath = lastPath;

                if (wanderTimer > wanderInterval)
                {

                    if (!ProtectPosition)
                        Destroy(gameObject);
                    else
                    {
                        float angle = (Random.value - Random.value) * wanderDegreesDelta;
                        Vector3 direction = Quaternion.AngleAxis(angle, Vector3.up) * lastWanderDirection.normalized;
                        Vector3 circleCenter = ProtectPosition.position + lastPath;
                        circleCenter.y = transform.position.y;
                        Vector3 destination = circleCenter + direction.normalized;
                        targetPath = destination - transform.position;
                        targetPath = targetPath.normalized * maxSpeed;



                        //Debug.DrawRay(r.origin, r.direction, Color.red, wanderInterval);



                        lastPath = targetPath;
                        lastWanderDirection = direction;
                        wanderTimer = 0;
                    }

                }
            }
            else if (avoiding)
            {

                targetPath = avoidPath;

            }
            else
            {
                if (HeroObject)
                    targetPath = HeroObject.transform.position - transform.position;
                else
                    HeroObject = null;
            }
        }
    }


    public Vector3 GetTargetPath()
    {
        if (foundPath)
        {
            agent.Pathfind(this);
            targetPath = agent.targetPath;
        }

        return targetPath;
    }
    private void captureCheck()
    {


        Collider[] rangeChecks = Physics.OverlapSphere(transform.position, captureRadius, agent.targetMask);


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
                if (!Physics.Raycast(transform.position, direction, dist, agent.blockedMask))
                {
                    GameInfoScript.Instance.HeroCaptured(rangeChecks[i].gameObject);
                    //Destroy(rangeChecks[0].gameObject);
                    //SceneManager.LoadScene("Task 10");
                }

            }




        }
    }
    private void viewCheck()
    {
        if (ProtectPosition)
        {
            if ((ProtectPosition.position - transform.position).magnitude < 5)
            {
                foundPath = false;
                pathPoints.Clear();
            }
        }

        Collider[] rangeChecks = Physics.OverlapSphere(transform.position, radius, agent.targetMask);

        //the cone extents
        Vector3 angle1 = h.DirectionFromAngle(transform.eulerAngles.y, -angle / 2f);
        Vector3 angle2 = h.DirectionFromAngle(transform.eulerAngles.y, angle / 2f);
        //

        viewLine1 = (transform.position - (transform.position + angle1 * radius)).normalized;
        viewLine2 = (transform.position - (transform.position + angle2 * radius)).normalized;

        extentP1 = transform.position + angle1 * radius;
        extentP2 = transform.position + angle2 * radius;


        /*
                    minimum value of t will be 0

                    maximum value of t is whichever t that gives me the extentP1

                    extentP1.x = transform.position.x + viewLine1.x * t
                    extentP1.y = transform.position.y + viewLine1.y * t
                    extentP1.z = transform.position.z + viewLine1.z * t
                    



                    maxT1 = (extentP1 - transform.position) / viewLine1 
         */


        maxT1 = (extentP1.x - transform.position.x) / (viewLine1.x);
        maxT2 = (extentP2.x - transform.position.x) / (viewLine2.x);



        //If it detects something in its Circle
        if (rangeChecks.Length != 0)
        {
            GameObject target;
            Vector3 direction = Vector3.zero;
            List<GameObject> visibleHeroes = new List<GameObject>();
            for (int i = 0; i < rangeChecks.Length; i++)
            {

                target = rangeChecks[i].gameObject;
                direction = (target.transform.position - transform.position).normalized;
                if (Vector3.Angle(transform.forward, direction) < angle / 2f)
                    visibleHeroes.Add(target);



            }
            if (visibleHeroes.Count > 0)
            {
                float mag;
                float shortest = -1;
                int index = 0;

                for (int i = 0; i < visibleHeroes.Count; i++)
                {
                    mag = (transform.position - visibleHeroes[i].transform.position).magnitude;
                    if (shortest == -1)
                    {
                        shortest = mag;
                        index = 0;
                    }
                    else if (mag <= shortest)
                    {
                        shortest = mag;
                        index = i;
                    }
                }

                /*            }
                            //If the detected thing is within the Cone of vision
                            if (Vector3.Angle(transform.forward, direction) < angle / 2f)
                            {*/
                float dist = Vector3.Distance(transform.position, visibleHeroes[index].transform.position);
                HeroObject = visibleHeroes[index];
                hs = visibleHeroes[index].GetComponent<HeroScript10>();
                //Handles.color = Color.yellow;

                //If the detected thing isn't blocked by a Wall
                if (!Physics.Raycast(transform.position, direction, dist, agent.blockedMask))
                {
                    //If it hasn't already detected the player previously
                    if (!foundPlayer)
                    {

                        hs.detected = true;
                        if (hs.HoldingObjects.Count == 0 && !hs.escape)
                            hs.GetPathToBase();
                        hs.detectedBy.Add(gameObject);

                    }
                    wander = false;
                    foundPlayer = true;

                }
                else //If the detected thing is blocked by a wall
                {

                    //If the detected thing is blocked by a wall but the player was previously detected
                    if (foundPlayer)
                    {

                        if ((ProtectPosition.position - transform.position).magnitude >= 5 && !foundPath)
                            foundPath = pf.Prelim(ProtectPosition.position, out pathPoints, false);

                        if (hs)
                            hs.StartCoroutine(hs.fleeState(gameObject));
                    }



                    foundPlayer = false;

                    wander = true;
                }
            }
            else //If the detected thing isn't within the Cone of vision
            {
                if (foundPlayer)
                {
                    if (hs)
                        hs.StartCoroutine(hs.fleeState(gameObject));
                }

                wander = true;
                foundPlayer = false;
            }
        }
        else if (foundPlayer) //If nothing was detected in the circle, and the player was previously found
        {
            /*            hs.detected = false;
                        hs.detectedBy.Remove(gameObject);*/
            if ((ProtectPosition.position - transform.position).magnitude >= 5 && !foundPath)
                foundPath = pf.Prelim(ProtectPosition.position, out pathPoints, false);

            if (hs)
                hs.StartCoroutine(hs.fleeState(gameObject));
            wander = true;
            foundPlayer = false;
        }

        if (!foundPlayer)
        {
            Collider[] collisionChecks = Physics.OverlapSphere(transform.position, radius);
            //Check collision with other guardians
            for (int i = 0; i < collisionChecks.Length; i++)
            {
                if (collisionChecks[i].tag == "Guardian" && collisionChecks[i].gameObject != gameObject)
                {
                    if ((transform.position - collisionChecks[i].transform.position).magnitude <= 3f)
                    {
                        avoidPath = transform.position - collisionChecks[i].transform.position;
                        wander = false;
                        avoiding = true;
                        StartCoroutine(avoidCollision());
                    }
                }
                else if (collisionChecks[i].tag == "Blocker")
                {
                    if ((ProtectPosition.position - transform.position).magnitude >= 10)
                    {
                        if (Physics.Raycast(transform.position, (ProtectPosition.position - transform.position), (ProtectPosition.position - transform.position).magnitude, agent.blockedMask))
                        {
                            if (!foundPath)
                            {
                                foundPath = pf.Prelim(ProtectPosition.position, out pathPoints, false);
                            }
                        }
                    }
                }
            }


        }

    }
    private void OnDestroy()
    {
        BaseHit();
        GameInfoScript.Instance.GuardianDestroyed();
    }

    public void BaseHit()
    {
        if (hs)
        {
            hs.detectedBy.Remove(gameObject);

            if (hs.detectedBy.Count == 0)
            {
                hs.detected = false;
                hs.foundPath = false;
                hs.pathPoints.Clear();
                hs.GetClosestPrisoner();
                /*            if (hs.strat == HeroScript7.Strategy.Hunt)
                            {
                                hs.foundPath = false;
                                hs.pathPoints.Clear();
                                hs.GetClosestPrisoner();
                            }*/
            }
            agent.enabled = false;
            Destroy(gameObject);
        }
    }
    private IEnumerator avoidCollision()
    {
        yield return new WaitForSeconds(1);
        avoiding = false;
        wander = true;
    }
    private IEnumerator viewRoutine()
    {

        while (true)
        {

            yield return new WaitForSeconds(SearchDelay);
            viewCheck();
            captureCheck();
        }
    }
}
