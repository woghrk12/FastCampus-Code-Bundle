using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(FieldOfView))]
public class FieldOfView_Editor : Editor
{
    private void OnSceneGUI()
    {
        FieldOfView t_fov = (FieldOfView)target;

        Handles.color = Color.white;
        Handles.DrawWireArc(t_fov.transform.position, Vector3.up, Vector3.forward, 360, t_fov.viewRadius);

        Vector3 t_viewAngleA = t_fov.DirFromAngle(-t_fov.viewAngle * 0.5f, false);
        Vector3 t_viewAngleB = t_fov.DirFromAngle(t_fov.viewAngle * 0.5f, false);

        Handles.DrawLine(t_fov.transform.position, t_fov.transform.position + t_viewAngleA * t_fov.viewRadius);
        Handles.DrawLine(t_fov.transform.position, t_fov.transform.position + t_viewAngleB * t_fov.viewRadius);

        Handles.color = Color.red;
        foreach (Transform t_visibleTarget in t_fov.VisibleTargets)
            Handles.DrawLine(t_fov.transform.position, t_visibleTarget.position);
    }


}
