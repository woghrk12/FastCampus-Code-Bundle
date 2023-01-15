using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TopDownCamera))]
public class TopDownCamera_Editor : Editor
{
    #region Variables

    private TopDownCamera targetCamera = null;

    #endregion Variables

    public override void OnInspectorGUI()
    {
        targetCamera = (TopDownCamera)target;
        base.OnInspectorGUI();
    }

    private void OnSceneGUI()
    {
        if (!targetCamera || !targetCamera.Target) return;

        Transform t_target = targetCamera.Target;
        Vector3 t_targetPos = t_target.position;
        t_targetPos.y += targetCamera.lookAtHeight;

        Handles.color = new Color(1f, 0f, 0f, 0.15f);
        Handles.DrawSolidDisc(t_targetPos, Vector3.up, targetCamera.distance);

        Handles.color = new Color(0f, 1f, 0f, 0.75f);
        Handles.DrawWireDisc(t_targetPos, Vector3.up, targetCamera.distance);
        
        Handles.color = new Color(1f, 0f, 0f, 0.5f);
        targetCamera.distance = Handles.ScaleSlider(
            targetCamera.distance, 
            t_targetPos, 
            -t_target.forward, 
            Quaternion.identity, 
            targetCamera.distance, 
            0.1f);
        targetCamera.distance = Mathf.Clamp(targetCamera.distance, 2f, float.MaxValue);

        Handles.color = new Color(0f, 0f, 1f, 0.5f);
        targetCamera.height = Handles.ScaleSlider(
            targetCamera.height,
            t_targetPos,
            Vector3.up,
            Quaternion.identity,
            targetCamera.height,
            0.1f);
        targetCamera.height = Mathf.Clamp(targetCamera.height, 2f, float.MaxValue);

        GUIStyle t_labelStyle = new GUIStyle();
        t_labelStyle.fontSize = 15;
        t_labelStyle.normal.textColor = Color.white;
        t_labelStyle.alignment = TextAnchor.UpperCenter;

        Handles.Label(t_targetPos + (-t_target.forward * targetCamera.distance), "Distance", t_labelStyle);

        t_labelStyle.alignment = TextAnchor.MiddleRight;

        Handles.Label(t_targetPos + (Vector3.up * targetCamera.height), "Height", t_labelStyle);

        targetCamera.HandleCamera();
    }
}
