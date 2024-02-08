using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(HeroScript4))]
public class FoVHero4 : Editor
{
    void OnSceneGUI()
    {
        HeroScript4 gs = (HeroScript4)target;
        Helper h = new Helper();

        Handles.color = Color.white;
        Handles.DrawWireArc(gs.transform.position, Vector3.up, Vector3.forward, 360, gs.viewRadius);

        Handles.color = Color.cyan;
        Handles.DrawWireArc(gs.transform.position, Vector3.up, Vector3.forward, 360, gs.viewRadius / 4f);



        Vector3 viewAngle1 = h.DirectionFromAngle(gs.transform.eulerAngles.y, -gs.viewAngle / 2f);
        Vector3 viewAngle2 = h.DirectionFromAngle(gs.transform.eulerAngles.y, gs.viewAngle / 2f);

        Handles.color = Color.yellow;
        Handles.DrawLine(gs.transform.position, gs.transform.position + viewAngle1 * gs.viewRadius);
        Handles.DrawLine(gs.transform.position, gs.transform.position + viewAngle2 * gs.viewRadius);

    }


}


