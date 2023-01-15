using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CoverLookUp : MonoBehaviour
{
    private List<Vector3[]> allCoverSpots;
    private GameObject[] covers;
    private List<int> coverHashCodes;

    private Dictionary<float, Vector3> filteredSpots;


    private GameObject[] GetObjectsInLayerMask(int p_layerMask)
    {
        List<GameObject> t_return = new List<GameObject>();

        foreach (GameObject t_obj in Resources.FindObjectsOfTypeAll<GameObject>())
        {
            if (t_obj.activeInHierarchy && p_layerMask == (p_layerMask | 1 << t_obj.layer)) t_return.Add(t_obj);
        }

        return t_return.ToArray();
    }

    private void ProcessPoint(List<Vector3> p_points, Vector3 p_nativePoint, float p_range)
    {
        if (NavMesh.SamplePosition(p_nativePoint, out NavMeshHit t_hitInfo, p_range, NavMesh.AllAreas)) p_points.Add(t_hitInfo.position);
    }

    private Vector3[] GetSpots(GameObject p_obj, LayerMask p_obstacleMask)
    {
        List<Vector3> t_bounds = new List<Vector3>();

        foreach (Collider t_col in p_obj.GetComponents<Collider>())
        {
            float t_baseHeight = (t_col.bounds.center - t_col.bounds.extents).y;
            float t_range = 2 * t_col.bounds.extents.y;

            Vector3 t_destLocalForward = p_obj.transform.forward * p_obj.transform.localScale.z * 0.5f;
            Vector3 t_destLocalRight = p_obj.transform.right * p_obj.transform.localScale.x * 0.5f;

            if (p_obj.TryGetComponent(out MeshCollider t_meshCollider))
            {
                float t_maxBounds = t_meshCollider.bounds.extents.z + t_meshCollider.bounds.extents.x;
                Vector3 t_originForward = t_col.bounds.center + p_obj.transform.forward * t_maxBounds;
                Vector3 t_originRight = t_col.bounds.center + p_obj.transform.right * t_maxBounds;

                if (Physics.Raycast(t_originForward, t_col.bounds.center - t_originForward, out RaycastHit t_hitInfo, t_maxBounds, p_obstacleMask))
                    t_destLocalForward = t_hitInfo.point - t_col.bounds.center;
                if (Physics.Raycast(t_originRight, t_col.bounds.center - t_originRight, out t_hitInfo, t_maxBounds, p_obstacleMask))
                    t_destLocalRight = t_hitInfo.point - t_col.bounds.center;
            }
            else if (Vector3.Equals(p_obj.transform.localScale, Vector3.one))
            {
                t_destLocalForward = p_obj.transform.forward * t_col.bounds.extents.z;
                t_destLocalRight = p_obj.transform.right * t_col.bounds.extents.x;
            }

            float t_edgeFactor = 0.75f;
            ProcessPoint(t_bounds, t_col.bounds.center + t_destLocalRight + t_destLocalForward * t_edgeFactor, t_range);
            ProcessPoint(t_bounds, t_col.bounds.center + t_destLocalForward + t_destLocalRight * t_edgeFactor, t_range);
            ProcessPoint(t_bounds, t_col.bounds.center + t_destLocalForward, t_range);
            ProcessPoint(t_bounds, t_col.bounds.center + t_destLocalForward - t_destLocalRight * t_edgeFactor, t_range);
            ProcessPoint(t_bounds, t_col.bounds.center - t_destLocalRight + t_destLocalForward * t_edgeFactor, t_range);
            ProcessPoint(t_bounds, t_col.bounds.center + t_destLocalRight, t_range);
            ProcessPoint(t_bounds, t_col.bounds.center + t_destLocalRight - t_destLocalForward * t_edgeFactor, t_range);
            ProcessPoint(t_bounds, t_col.bounds.center - t_destLocalForward + t_destLocalRight * t_edgeFactor, t_range);
            ProcessPoint(t_bounds, t_col.bounds.center - t_destLocalForward, t_range);
            ProcessPoint(t_bounds, t_col.bounds.center - t_destLocalForward - t_destLocalRight * t_edgeFactor, t_range);
            ProcessPoint(t_bounds, t_col.bounds.center - t_destLocalRight - t_destLocalForward * t_edgeFactor, t_range);
            ProcessPoint(t_bounds, t_col.bounds.center - t_destLocalRight, t_range);
        }

        return t_bounds.ToArray();
    }

    public void SetUp(LayerMask p_coverMask)
    {
        covers = GetObjectsInLayerMask(p_coverMask);
        
        coverHashCodes = new List<int>();
        allCoverSpots = new List<Vector3[]>();

        foreach (GameObject t_coverObj in covers)
        {
            allCoverSpots.Add(GetSpots(t_coverObj, p_coverMask));
            coverHashCodes.Add(t_coverObj.GetHashCode());
        }
    }

    private bool TargetInPath(Vector3 p_origin, Vector3 p_spot, Vector3 p_target, float p_angle)
    {
        Vector3 t_dirToTarget = (p_target - p_origin).normalized;
        Vector3 t_dirToSpot = (p_spot - p_origin).normalized;

        if (Vector3.Angle(t_dirToSpot, t_dirToTarget) <= p_angle)
            return (p_target - p_origin).sqrMagnitude <= (p_spot - p_origin).sqrMagnitude;
        
        return false;
    }

    private ArrayList FilterSpots(StateController p_controller)
    {
        filteredSpots = new Dictionary<float, Vector3>();

        float t_minDist = Mathf.Infinity;
        int t_nextCoverHash = -1;
        for (int i = 0; i < allCoverSpots.Count; i++)
        {
            if (!covers[i].activeSelf || coverHashCodes[i] == p_controller.coverHash) continue;

            foreach (Vector3 t_spot in allCoverSpots[i])
            {
                Vector3 t_vectorDist = p_controller.personalTarget - t_spot;
                float t_searchDist = (p_controller.transform.position - t_spot).sqrMagnitude;

                if (t_vectorDist.sqrMagnitude <= p_controller.viewRadius * p_controller.viewRadius
                    && Physics.Raycast(t_spot, t_vectorDist, out RaycastHit t_hitInfo, t_vectorDist.sqrMagnitude, p_controller.generalStats.coverMask))
                {
                    if (t_hitInfo.collider == covers[i].GetComponent<Collider>()
                        && !TargetInPath(p_controller.transform.position, t_spot, p_controller.personalTarget, p_controller.viewAngle / 4))
                    {
                        if (!filteredSpots.ContainsKey(t_searchDist)) filteredSpots.Add(t_searchDist, t_spot);
                        else continue;

                        if (t_minDist > t_searchDist)
                        {
                            t_minDist = t_searchDist;
                            t_nextCoverHash = coverHashCodes[i];
                        }
                    }
                }
            }
        }

        ArrayList t_returnArray = new ArrayList();
        t_returnArray.Add(t_nextCoverHash);
        t_returnArray.Add(t_minDist);

        return t_returnArray;
    }

    public ArrayList GetBestCoverSpot(StateController p_controller)
    {
        ArrayList t_nextCoverData = FilterSpots(p_controller);
        int t_nextCoverHash = (int)t_nextCoverData[0];
        float t_minDist = (float)t_nextCoverData[1];

        ArrayList t_returnArray = new ArrayList();
        if (filteredSpots.Count == 0)
        {
            t_returnArray.Add(-1);
            t_returnArray.Add(Vector3.positiveInfinity);
        }
        else
        {
            t_returnArray.Add(t_nextCoverHash);
            t_returnArray.Add(filteredSpots[t_minDist]);
        }

        return t_returnArray;
    }
}
