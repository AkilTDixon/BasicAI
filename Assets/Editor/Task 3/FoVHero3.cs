using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(HeroScript3))]
public class FoVHero3 : Editor
{
    void OnSceneGUI()
    {
        HeroScript3 gs = (HeroScript3)target;

        Handles.color = Color.white;
        Handles.DrawWireArc(gs.transform.position, Vector3.up, Vector3.forward, 360, gs.viewRadius);

        Handles.color = Color.cyan;
        Handles.DrawWireArc(gs.transform.position, Vector3.up, Vector3.forward, 360, gs.viewRadius / 4f);



        Vector3 viewAngle1 = DirectionFromAngle(gs.transform.eulerAngles.y, -gs.viewAngle / 2f);
        Vector3 viewAngle2 = DirectionFromAngle(gs.transform.eulerAngles.y, gs.viewAngle / 2f);

        Handles.color = Color.yellow;
        Handles.DrawLine(gs.transform.position, gs.transform.position + viewAngle1 * gs.viewRadius);
        Handles.DrawLine(gs.transform.position, gs.transform.position + viewAngle2 * gs.viewRadius);

    }


    private Vector3 DirectionFromAngle(float eulerY, float angleInDegrees)
    {
        angleInDegrees += eulerY;

        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }
}


