using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;

public class GuardianScript2 : MonoBehaviour
{
    [SerializeField] public GameObject HeroObject;
    [SerializeField] private float SearchDelay = 0.2f;
    [SerializeField] private Transform ProtectPosition;

    private Helper h = new Helper();
    public float angle, radius;

    public LayerMask targetMask;
    public LayerMask blockedMask;

    private bool foundPlayer;

    public float wanderDegreesDelta = 45;
    [Min(0)] public float wanderInterval = 0.75f;
    protected float wanderTimer = 0;

    private Vector3 lastWanderDirection;
    private Vector3 lastPath;

    private Vector3 targetPath;
    public bool wander = false;
    private HeroScript2 hs;
    private float maxSpeed;

    public Vector3 viewLine1;
    public Vector3 viewLine2;
    public Vector3 extentP1;
    public Vector3 extentP2;
    public float angle1, angle2;

    public float maxT1, maxT2, minT1, minT2;

    void Start()
    {
        maxSpeed = GetComponent<AIBaseScript2>().maxSpeed;
        hs = HeroObject.GetComponent<HeroScript2>();
        StartCoroutine(viewRoutine());
    }

    void Update()
    {
        if (hs.victory)
            BaseHit();
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

                    lastPath = targetPath;
                    lastWanderDirection = direction;
                    wanderTimer = 0;
                }

            }
        }
        else
        {
            targetPath = HeroObject.transform.position - transform.position;
        }
    }


    public Vector3 GetTargetPath()
    {
        return targetPath;
    }
    
    private void viewCheck()
    {
        Collider[] rangeChecks = Physics.OverlapSphere(transform.position, radius, targetMask);
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
        if (rangeChecks.Length != 0)
        {
            Transform target = rangeChecks[0].transform;

            Vector3 direction = (target.position - transform.position).normalized;

            if (Vector3.Angle(transform.forward, direction) < angle / 2f)
            {
                float dist = Vector3.Distance(transform.position, target.position);

                Handles.color = Color.yellow;


                if (!Physics.Raycast(transform.position, direction, dist, blockedMask))
                {
                    if (!foundPlayer)
                    {

                        hs.detected = true;
                        hs.detectedBy.Add(gameObject);

                    }
                    wander = false;
                    foundPlayer = true;

                }
                else
                {

                    if (foundPlayer)
                    {
                        hs.StartCoroutine(hs.fleeState(gameObject));
                    }



                    foundPlayer = false;

                    wander = true;
                }
            }
            else
            {
                if (foundPlayer)
                {

                    hs.StartCoroutine(hs.fleeState(gameObject));
                }

                wander = true;
                foundPlayer = false;
            }
        }
        else if (foundPlayer)
        {
            /*            hs.detected = false;
                        hs.detectedBy.Remove(gameObject);*/
            hs.StartCoroutine(hs.fleeState(gameObject));
            wander = true;
            foundPlayer = false;
        }

    }


    public void BaseHit()
    {
        hs.detectedBy.Remove(gameObject);

        if (hs.detectedBy.Count == 0)
            hs.detected = false;

        Destroy(gameObject);
    }
    private IEnumerator viewRoutine()
    {

        while (true)
        {

            yield return new WaitForSeconds(SearchDelay);
            viewCheck();
        }
    }
}
