using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GuardianScript4))]
public class FoV4 : Editor
{
    void OnSceneGUI()
    {

        Helper h = new Helper();
        GuardianScript4 gs = (GuardianScript4)target;

        Handles.color = Color.white;
        Handles.DrawWireArc(gs.transform.position, Vector3.up, Vector3.forward, 360, gs.radius);

        Handles.color = Color.cyan;
        Handles.DrawWireArc(gs.transform.position, Vector3.up, Vector3.forward, 360, gs.radius / 4f);


        
        Vector3 viewAngle1 = h.DirectionFromAngle(gs.transform.eulerAngles.y, -gs.angle / 2f);
        Vector3 viewAngle2 = h.DirectionFromAngle(gs.transform.eulerAngles.y, gs.angle / 2f);

        Handles.color = Color.blue;
        Handles.DrawLine(gs.transform.position, gs.transform.position + viewAngle1 * gs.radius);
        Handles.color = Color.green;
        Handles.DrawLine(gs.transform.position, gs.transform.position + viewAngle2 * gs.radius);

    }

}


