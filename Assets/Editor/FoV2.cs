using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GuardianScript2))]
public class FoV2 : Editor
{
    void OnSceneGUI()
    {
        GuardianScript2 gs = (GuardianScript2)target;

        Handles.color = Color.white;
        Handles.DrawWireArc(gs.transform.position, Vector3.up, Vector3.forward, 360, gs.radius);

        Handles.color = Color.cyan;
        Handles.DrawWireArc(gs.transform.position, Vector3.up, Vector3.forward, 360, gs.radius / 4f);



        Vector3 viewAngle1 = DirectionFromAngle(gs.transform.eulerAngles.y, -gs.angle / 2f);
        Vector3 viewAngle2 = DirectionFromAngle(gs.transform.eulerAngles.y, gs.angle / 2f);

        Handles.color = Color.yellow;
        Handles.DrawLine(gs.transform.position, gs.transform.position + viewAngle1 * gs.radius);
        Handles.DrawLine(gs.transform.position, gs.transform.position + viewAngle2 * gs.radius);

    }


    private Vector3 DirectionFromAngle(float eulerY, float angleInDegrees)
    {
        angleInDegrees += eulerY;

        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }
}


