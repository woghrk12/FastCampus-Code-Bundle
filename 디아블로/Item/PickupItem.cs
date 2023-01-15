using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupItem : MonoBehaviour, IInteractable
{
    [SerializeField] private float distance = 3f;
    public float Distance => distance;

    public ItemObject itemObject;
    public int amount;

    public bool Interact(GameObject p_otherObj)
    {
        float t_calcDist = Vector3.Distance(transform.position, p_otherObj.transform.position);
        if (t_calcDist > distance) return false;
        return p_otherObj.GetComponent<PlayerController>()?.PickupItem(this) ?? false;
    }

    public void StopInteract(GameObject p_otherObj)
    {
        throw new System.NotImplementedException();
    }

    private void OnValidate()
    {
#if UNITY_EDITOR
        GetComponent<SpriteRenderer>().sprite = itemObject?.icon;
        GetComponentInChildren<TextMesh>().text = itemObject ? itemObject.name + amount.ToString() : null;
#endif
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, distance);
    }
}
