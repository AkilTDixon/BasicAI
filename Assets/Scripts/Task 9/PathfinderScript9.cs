using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathfinderScript9 : MonoBehaviour
{

    [SerializeField] private LayerMask tarMask1;
    [SerializeField] private LayerMask tarMask2;
    [SerializeField] public float rayDistance = 2f;
    [SerializeField] public float castWidth = 2f;
    [SerializeField] public bool debugToggle = false;

    private LayerMask tarMask;
    private Vector3 targetPos;
    private AIBaseScript9 agent;
    private BoxCollider col;

    // Start is called before the first frame update
    void Start()
    {
        col = GetComponent<BoxCollider>();
        agent = GetComponent<AIBaseScript9>();
    }

    public bool Prelim(Vector3 targ, out List<Vector3> pathPoints, bool guardian)
    {

        if (guardian)
            tarMask = tarMask2;
        else
            tarMask = tarMask1;

        pathPoints = new List<Vector3>();
        List<Vector3> leftPath = new List<Vector3>();
        List<Vector3> rightPath = new List<Vector3>();
        targetPos = targ;
        Vector3 dir = (targetPos - transform.position).normalized;
        Ray r = new Ray(transform.position, dir);
        bool retRight = false;
        bool retLeft = false;

        float magRight = 0;
        float magLeft = 0;

        int depthRight = 0;
        int depthLeft = 0;

        Vector3 initialPos = transform.position;
        RaycastHit h;
        if (Physics.Raycast(transform.position, (targ - transform.position), out h, rayDistance, agent.blockedMask))
        {
            initialPos = transform.position + h.normal * 5f;
            agent.Velocity = Vector3.zero;
        }

        rightPath.Add(initialPos);
        leftPath.Add(initialPos);


        //rayPathFavor = true; keep to the right
        //rayPathFavor = false; keep to the left
        retRight = recursivePath(r, rightPath, false, false, false, ref depthRight, true);
        if (retRight)
        {

            for (int i = 0; i < rightPath.Count - 1; i++)
                magRight += (rightPath[i] - rightPath[i + 1]).magnitude;

        }
        retLeft = recursivePath(r, leftPath, false, false, false, ref depthLeft, false);
        if (retLeft)
        {
            for (int i = 0; i < leftPath.Count - 1; i++)
                magLeft += (leftPath[i] - leftPath[i + 1]).magnitude;

        }



        if (magRight == 0 || !retRight)
            pathPoints = leftPath;
        else if (magLeft == 0 || !retLeft)
            pathPoints = rightPath;
        else if (magRight <= magLeft)
            pathPoints = rightPath;
        else
            pathPoints = leftPath;

        if (debugToggle)
            for (int i = 0; i < pathPoints.Count - 1; i++)
                Debug.DrawLine(pathPoints[i], pathPoints[i + 1]);

        if (retRight)
            return retRight;
        else if (retLeft)
            return retLeft;
        else
            return false;

    }

    //Non-recursive function
    public bool StartPath(Vector3 targetPosition, out List<Vector3> pathPoints)
    {



        bool foundPrisoner = false;
        pathPoints = new List<Vector3>();

        bool detourBG = false;

        Vector3 dir = (targetPosition - transform.position).normalized;
        Ray r = new Ray(transform.position, dir);
        RaycastHit hitPrisoner;
        RaycastHit hitBlock;
        RaycastHit hitRight;
        RaycastHit hitLeft;
        pathPoints.Add(transform.position);

        int count = 0;

        //Shoot a ray toward the target
        //If a blocker isnt hit in that direction, move in that direction
        //Else, try a new direction


        while (!foundPrisoner)
        {
            //If caught in a loop
            if (count >= 20000)
                break;

            if (Physics.SphereCast(r.origin, castWidth, r.direction, out hitPrisoner, rayDistance, tarMask, QueryTriggerInteraction.Collide))
            {
                if (hitPrisoner.collider.tag == "Prisoner" || hitPrisoner.collider.tag == "Base")
                {
                    pathPoints.Add(r.origin + r.direction * (r.origin - hitPrisoner.point).magnitude);
                    if (debugToggle)
                    {
                        Debug.DrawRay(r.origin, r.direction * (r.origin - hitPrisoner.point).magnitude, Color.green);
                        Debug.Break();
                    }
                    foundPrisoner = true;
                    break;
                }
            }


            if (!Physics.SphereCast(r, castWidth, out hitBlock, rayDistance, agent.blockedMask))
            {
                if (detourBG)
                {
                    pathPoints.Add(r.origin);
                    detourBG = false;
                }
                if (debugToggle)
                    Debug.DrawLine(r.origin, r.origin + r.direction * rayDistance, Color.red);
                r.origin = r.origin + r.direction * rayDistance;
                r.direction = (targetPosition - r.origin).normalized;

                if (debugToggle)
                    Debug.Break();
            }
            else if (!Physics.SphereCast(r.origin, castWidth, Vector3.RotateTowards(hitBlock.normal, hitBlock.transform.forward, 90, 0), out hitRight, rayDistance, agent.blockedMask))
            {
                detourBG = true;
                Vector3 d = Vector3.RotateTowards(hitBlock.normal, hitBlock.transform.forward, 90, 0);
                if (debugToggle)
                    Debug.DrawLine(r.origin, r.origin + d * rayDistance, Color.blue);

                r.origin = r.origin + hitBlock.transform.forward * rayDistance;
                r.direction = (targetPosition - r.origin).normalized;
                if (debugToggle)
                    Debug.Break();
            }
            else if (!Physics.SphereCast(r.origin, castWidth, Vector3.RotateTowards(hitBlock.normal, -hitBlock.transform.forward, 90, 0), out hitLeft, rayDistance, agent.blockedMask))
            {
                detourBG = true;
                Vector3 d = Vector3.RotateTowards(hitBlock.normal, -hitBlock.transform.forward, 90, 0);

                if (debugToggle)
                    Debug.DrawLine(r.origin, r.origin + d * rayDistance, Color.green);
                r.origin = r.origin + (-hitBlock.transform.forward) * rayDistance;
                r.direction = (targetPosition - r.origin).normalized;
                if (debugToggle)
                    Debug.Break();
            }

            count++;
        }
        if (debugToggle)
            for (int i = 0; i < pathPoints.Count - 1; i++)
                Debug.DrawLine(pathPoints[i], pathPoints[i + 1]);

        return foundPrisoner;

    }


    private bool recursivePath(Ray r, List<Vector3> pathPoints, bool direct, bool right, bool left, ref int depth, bool rayPathFavor)
    {

        RaycastHit hitPrisoner, hitBlock, hitRight, hitLeft;
        bool turn = false;


        if (depth >= 100)
            return false;

        //base case
        if (Physics.SphereCast(r.origin, castWidth, r.direction, out hitPrisoner, rayDistance, tarMask, QueryTriggerInteraction.Collide))
        {
            if (hitPrisoner.collider.tag == "Prisoner" || hitPrisoner.collider.tag == "Base" || hitPrisoner.collider.tag == "Guardian")
            {
                if (!pathPoints.Contains(r.origin + r.direction * (r.origin - hitPrisoner.point).magnitude))
                    pathPoints.Add(r.origin + r.direction * (r.origin - hitPrisoner.point).magnitude);
                if (debugToggle)
                {
                    Debug.DrawRay(r.origin, r.direction * (r.origin - hitPrisoner.point).magnitude, Color.green);
                    Debug.Break();
                }
                return true;
            }
        }





        /*
         3 'routes'

        The direct route
        if (!Physics.SphereCast(r.origin, castWidth, r.direction, out hitBlock, rayDistance, agent.blockedMask))

        The right route
        else if (!Physics.SphereCast(r.origin, castWidth, Vector3.RotateTowards(hitBlock.normal, hitBlock.transform.forward,90,0), out hitRight, rayDistance, agent.blockedMask))

        The left route
        else if (!Physics.SphereCast(r.origin, castWidth, Vector3.RotateTowards(hitBlock.normal, -hitBlock.transform.forward, 90, 0), out hitLeft, rayDistance, agent.blockedMask))

         */
        if (direct)
            turn = true;
        bool dray = false;
        direct = Physics.SphereCast(r.origin, castWidth, r.direction, out hitBlock, rayDistance, agent.blockedMask);
        

        if (!direct)
        {
            if (turn)
                pathPoints.Add(r.origin);

            if (debugToggle)
                Debug.DrawLine(r.origin, r.origin + r.direction * rayDistance, Color.red);

            r.origin = r.origin + r.direction * rayDistance;
            r.direction = (targetPos - r.origin).normalized;

            if (debugToggle)
                Debug.Break();
            depth++;
            return recursivePath(r, pathPoints, direct, right, left, ref depth, rayPathFavor);

        }
        else
        {

            if (rayPathFavor)
            {

                right = Physics.SphereCast(r.origin, castWidth, Vector3.RotateTowards(hitBlock.normal, hitBlock.transform.forward, 90, 0), out hitRight, rayDistance, agent.blockedMask);
                
                if (!right)
                {
                    Vector3 d = Vector3.RotateTowards(hitBlock.normal, hitBlock.transform.forward, 90, 0);
                    if (debugToggle)
                    {
                        Debug.DrawLine(r.origin, r.origin + d * rayDistance, Color.blue);
   
                    }
                    r.origin = r.origin + Vector3.RotateTowards(hitBlock.normal, hitBlock.transform.forward, 90, 0) * rayDistance;
                    r.direction = (targetPos - r.origin).normalized;

                    if (debugToggle)
                        Debug.Break();

                    depth++;
                    return recursivePath(r, pathPoints, direct, right, left, ref depth, rayPathFavor);
                }
                else
                {
                    bool behind = Physics.SphereCast(r.origin, castWidth, -transform.forward, out hitRight, rayDistance, agent.blockedMask);
                    if (!behind)
                    {
                        Vector3 d = -transform.forward;
                        if (debugToggle)
                        {
                            Debug.DrawLine(r.origin, r.origin + d * rayDistance, Color.black);

                        }
                        r.origin = r.origin + d * rayDistance;
                        r.direction = (targetPos - r.origin).normalized;

                        if (debugToggle)
                            Debug.Break();

                        depth++;
                        return recursivePath(r, pathPoints, direct, behind, left, ref depth, rayPathFavor);
                    }
                    else
                        return false;
                }
                    
            }
            else
            {

                left = Physics.SphereCast(r.origin, castWidth, Vector3.RotateTowards(hitBlock.normal, -hitBlock.transform.forward, 90, 0), out hitLeft, rayDistance, agent.blockedMask);
                

                if (!left)
                {
                    Vector3 d = Vector3.RotateTowards(hitBlock.normal, -hitBlock.transform.forward, 90, 0);
                    if (debugToggle)
                    {
                        Debug.DrawLine(r.origin, r.origin + d * rayDistance, Color.green);

                    }
                    r.origin = r.origin + Vector3.RotateTowards(hitBlock.normal, -hitBlock.transform.forward, 90, 0) * rayDistance;
                    r.direction = (targetPos - r.origin).normalized;

                    if (debugToggle)
                        Debug.Break();

                    depth++;
                    return recursivePath(r, pathPoints, direct, right, left, ref depth, rayPathFavor);
                }
                else
                {
                    bool behind = Physics.SphereCast(r.origin, castWidth, -transform.forward, out hitLeft, rayDistance, agent.blockedMask);
                    if (!behind)
                    {
                        Vector3 d = -transform.forward;
                        if (debugToggle)
                        {
                            Debug.DrawLine(r.origin, r.origin + d * rayDistance, Color.black);

                        }

                        r.origin = r.origin + d * rayDistance;
                        r.direction = (targetPos - r.origin).normalized;

                        if (debugToggle)
                            Debug.Break();

                        depth++;
                        return recursivePath(r, pathPoints, direct, right, behind, ref depth, rayPathFavor);
                    }
                    else
                        return false;
                }
                
            }

        }
        //return recursionTest(r,pathPoints,);
    }

}
