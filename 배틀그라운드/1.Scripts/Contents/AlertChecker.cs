using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlertChecker : MonoBehaviour
{
    [Range(0, 50)]public float alertRadius;
    public int extraWaves = 1;

    public LayerMask alertMask = FC.TagAndLayer.LayerMasking.Enemy;
    
    private Vector3 curPos;
    private bool isAlert;

    private void Start()
    {
        InvokeRepeating("PingAlert", 1f, 1f);
    }

    private void AlertNearBy(Vector3 p_origin, Vector3 p_target, int p_wave = 0)
    {
        if (p_wave > extraWaves) return;

        Collider[] t_targetInViewRadius = Physics.OverlapSphere(p_origin, alertRadius, alertMask);
        foreach (Collider t_obj in t_targetInViewRadius)
        {
            t_obj.SendMessageUpwards("AlertCallback", p_target, SendMessageOptions.DontRequireReceiver);
            AlertNearBy(t_obj.transform.position, p_target, p_wave + 1);
        }
    }

    public void RootAlertNearBy(Vector3 p_origin)
    {
        curPos = p_origin;
        isAlert = true;
    }

    private void PingAlert()
    {
        if (!isAlert) return;

        isAlert = false;
        AlertNearBy(curPos, curPos);
    }
}
