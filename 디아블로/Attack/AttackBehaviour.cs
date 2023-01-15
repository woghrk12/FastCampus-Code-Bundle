using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AttackBehaviour : MonoBehaviour
{
    #region Variables

#if UNITY_EDITOR
    [Multiline] public string developmentDescription = "";
#endif

    public int animIdx = 0;

    public int priority = 0;

    public int damage = 10;
    public float range = 3f;
    [SerializeField] protected float coolTime = 0f;
    protected float calcCoolTime = 0.0f;

    public GameObject effectPrefab = null;

    [HideInInspector] public LayerMask targetMask;
    public bool IsAvailable = true;

#endregion Variables

    // Start is called before the first frame update
    void Start()
    {
        calcCoolTime = coolTime;
    }

    // Update is called once per frame
    void Update()
    {
        if (calcCoolTime < coolTime)
        {
            calcCoolTime += Time.deltaTime;
        }
    }

    public abstract void ExecuteAttack(GameObject target = null, Transform startPoint = null);
}
