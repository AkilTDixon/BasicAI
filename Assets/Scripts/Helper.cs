using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Helper : MonoBehaviour
{
    public Vector3 DirectionFromAngle(float eulerY, float angleInDegrees)
    {
        angleInDegrees += eulerY;

        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }

    public int getHighestWeight(Vector3 v)
    {
        int ret = 0;

        if (Mathf.Abs(v.x) >= Mathf.Abs(v.y))
        {
            if (Mathf.Abs(v.x) >= Mathf.Abs(v.z))
                ret = 0;
        }
        else if (Mathf.Abs(v.y) >= Mathf.Abs(v.x))
        {
            if (Mathf.Abs(v.y) >= Mathf.Abs(v.z))
                ret = 1;
        }
        else if (Mathf.Abs(v.z) >= Mathf.Abs(v.x))
        {
            if (Mathf.Abs(v.z) >= Mathf.Abs(v.y))
                ret = 2;
        }


        return ret;
    }
}
