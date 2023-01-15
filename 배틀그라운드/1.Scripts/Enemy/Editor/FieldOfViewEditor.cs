using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(StateController))]
public class FieldOfViewEditor : Editor
{
    private void OnSceneGUI()
    {
        StateController t_controller = target as StateController;

        if (t_controller == null || t_controller.gameObject == null) return;

        Handles.color = Color.white;
        Handles.DrawWireArc(t_controller.transform.position, Vector3.up, Vector3.forward, 360, t_controller.perceptionRadius);
        Handles.DrawWireArc(t_controller.transform.position, Vector3.up, Vector3.forward, 360, t_controller.perceptionRadius * 0.5f);

        Vector3 t_viewAngleA = DirectionFromAngle(t_controller.transform, -t_controller.viewAngle * 0.5f, false);
        Vector3 t_viewAngleB = DirectionFromAngle(t_controller.transform, t_controller.viewAngle * 0.5f, false);

        Handles.DrawWireArc(t_controller.transform.position, Vector3.up, t_viewAngleA, t_controller.viewAngle, t_controller.viewRadius);
        Handles.DrawLine(t_controller.transform.position, t_controller.transform.position + t_viewAngleA * t_controller.viewRadius);
        Handles.DrawLine(t_controller.transform.position, t_controller.transform.position + t_viewAngleB * t_controller.viewRadius);

        Handles.color = Color.yellow;
        if (t_controller.isTargetInsight && t_controller.personalTarget != Vector3.zero)
        {
            Handles.DrawLine(t_controller.enemyAnim.muzzleTr.position, t_controller.personalTarget);
        }
    }

    private Vector3 DirectionFromAngle(Transform p_transform, float p_angleInDegrees, bool p_isAngleGlobal)
    {
        if (!p_isAngleGlobal) p_angleInDegrees += p_transform.eulerAngles.y;

        return new Vector3(Mathf.Sin(p_angleInDegrees * Mathf.Deg2Rad), 0f, Mathf.Cos(p_angleInDegrees * Mathf.Deg2Rad));
    }
}
