using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldOfView : MonoBehaviour
{
    #region Variables

    public float viewRadius = 5f;
    [Range(0, 360)] public float viewAngle = 90f;

    [SerializeField] private LayerMask targetMask;
    [SerializeField] private LayerMask obstacleMask;

    private List<Transform> visibleTargets = new List<Transform>();
    public List<Transform> VisibleTargets => visibleTargets;
    private Transform nearestTarget = null;
    public Transform NearestTarget => nearestTarget;
    private float distToTarget = 0.0f;

    [SerializeField] private float delay = 0.2f;

    #endregion Variables

    private void Start()
    {
        StartCoroutine(FindTargetsWithDelay(delay));
    }

    private IEnumerator FindTargetsWithDelay(float delay)
    {
        while (true)
        {
            yield return new WaitForSeconds(delay);
            FindVisibleTargets();
        }
    }

    private void FindVisibleTargets()
    {
        distToTarget = 0.0f;
        nearestTarget = null;
        visibleTargets.Clear();

        Collider[] t_targetsInRadius = Physics.OverlapSphere(transform.position, viewRadius, targetMask);
        for (int idx = 0; idx < t_targetsInRadius.Length; idx++)
        {
            Transform t_target = t_targetsInRadius[idx].transform;
            Vector3 t_dirToTarget = (t_target.position - transform.position).normalized;
            if (Vector3.Angle(transform.forward, t_dirToTarget) > viewAngle * 0.5f) continue;

            float t_distToTarget = Vector3.Distance(transform.position, t_target.position);
            if (Physics.Raycast(transform.position, t_dirToTarget, t_distToTarget, obstacleMask)) continue;

            visibleTargets.Add(t_target);

            if (!(nearestTarget is null) && distToTarget <= t_distToTarget) continue;

            nearestTarget = t_target;
            distToTarget = t_distToTarget;
        }
    }

    public Vector3 DirFromAngle(float p_angleInDeg, bool p_angleIsGlobal)
    {
        if (!p_angleIsGlobal) p_angleInDeg += transform.eulerAngles.y;
        return new Vector3(Mathf.Sin(p_angleInDeg * Mathf.Deg2Rad), 0, Mathf.Cos(p_angleInDeg * Mathf.Deg2Rad));
    }
}
