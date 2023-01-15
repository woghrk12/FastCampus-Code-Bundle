using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;

public class PlayerController : MonoBehaviour
{
    #region Variables

    public TargetPicker picker = null;

    [SerializeField] private Animator anim = null;
    private CharacterController controller = null;
    private NavMeshAgent agent = null;
    private Camera mainCam = null;

    [SerializeField] private LayerMask groundLayerMask;

    private readonly int walkHash = Animator.StringToHash("Walk");

    public InventoryObject inventory;
    public InventoryObject equipment;

    private Transform target = null;

    [SerializeField] public StatObject playerStats;

    #endregion Variables

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        agent = GetComponent<NavMeshAgent>();

        agent.updatePosition = false;
        agent.updateRotation = true;

        mainCam = Camera.main;

        inventory.OnUseItem += OnUseItem;
    }

    private void Update()
    {
        bool t_flagOnUI = EventSystem.current.IsPointerOverGameObject();

        if (!t_flagOnUI && Input.GetMouseButtonDown(0))
        {
            Ray t_ray = mainCam.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(t_ray, out RaycastHit t_hitInfo, 100, groundLayerMask))
            {
                agent.SetDestination(t_hitInfo.point);
                if (picker) picker.SetPosition(t_hitInfo);
            }
        }

        if (!t_flagOnUI && Input.GetMouseButtonDown(1))
        {
            Ray t_ray = mainCam.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(t_ray, out RaycastHit t_hitInfo, 100))
            {
                IInteractable t_interactable = t_hitInfo.collider.GetComponent<IInteractable>();
                if (t_interactable != null)
                {
                    SetTarget(t_hitInfo.collider.transform, t_interactable.Distance);
                }
            }
        }

        if (agent.remainingDistance > agent.stoppingDistance)
        {
            controller.Move(agent.velocity * Time.deltaTime);
            anim.SetBool(walkHash, true);
        }
        else
        {
            controller.Move(agent.velocity * Time.deltaTime);
            if (!agent.pathPending)
            {
                anim.SetBool(walkHash, false);
                agent.ResetPath();
            }
            if (target != null)
            {
                if (target.GetComponent<IInteractable>() != null)
                {
                    IInteractable t_interactable = target.GetComponent<IInteractable>();
                    if (t_interactable.Interact(this.gameObject)) RemoveTarget();
                }
            }
        }
    }

    private void LateUpdate()
    {
        transform.position = agent.nextPosition;
    }

    private void OnTriggerEnter(Collider other)
    {
        GroundItem t_item = other.GetComponent<GroundItem>();
        if (t_item)
        {
            if (inventory.AddItem(new Item(t_item.itemObject), t_item.amount))
                Destroy(other.gameObject);
        }
    }

    private void SetTarget(Transform p_target, float p_stoppingDist)
    {
        target = p_target;

        agent.stoppingDistance = p_stoppingDist;
        agent.updateRotation = false;
        agent.SetDestination(target.position);

        if (picker) picker.target = target.transform;
    }

    private void RemoveTarget()
    {
        target = null;
        agent.stoppingDistance = 0f;
        agent.updateRotation = true;

        agent.ResetPath();
    }

    public bool PickupItem(PickupItem p_item, int p_amount = 1)
    {
        if (p_item.itemObject != null && inventory.AddItem(new Item(p_item.itemObject), p_amount))
        {
            Destroy(p_item.gameObject);
            return true;
        }

        return false;
    }

    private void OnUseItem(ItemObject p_item)
    {
        foreach (ItemBuff t_buff in p_item.data.buffs)
        {
            if (t_buff.stat == ECharacterAttribute.Health)
            {
                playerStats.AddHealth(t_buff.value);
            }
        }
    }
}
