using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

public class EnemyHealth : HealthBase
{
    public float health = 100f;
    public GameObject healthHUD;
    public GameObject bloodSample;
    public bool isHeadShot;

    private float totalHealth;
    private Transform weapon;
    private Transform hud;
    private RectTransform healthBar;
    private float originalBarScale;
    private HealthHUD healthUI;

    private StateController controller;
    private GameObject gameController;

    private void Awake()
    {
        hud = Instantiate(healthHUD, transform).transform;

        if (!hud.gameObject.activeSelf) hud.gameObject.SetActive(true);
        totalHealth = health;
        healthBar = hud.transform.Find("Bar").GetComponent<RectTransform>();
        healthUI = hud.GetComponent<HealthHUD>();
        originalBarScale = healthBar.sizeDelta.x;

        anim = GetComponent<Animator>();
        controller = GetComponent<StateController>();
        gameController = GameObject.FindGameObjectWithTag(FC.TagAndLayer.TagName.GameController);

        foreach (Transform t_childTr in anim.GetBoneTransform(HumanBodyBones.RightHand))
        {
            weapon = t_childTr.Find("Muzzle");
            if (weapon != null) break;
        }

        weapon = weapon.parent;
    }

    private void UpdateHealthBar()
    {
        float t_scaleFactor = health / totalHealth;
        healthBar.sizeDelta = new Vector2(t_scaleFactor * originalBarScale, healthBar.sizeDelta.y);
    }

    private void RemoveAllForces()
    {
        foreach (Rigidbody t_body in GetComponentsInChildren<Rigidbody>())
        {
            t_body.isKinematic = false;
            t_body.velocity = Vector3.zero;
        }
    }

    public void Die()
    {
        foreach (MonoBehaviour t_behaviour in GetComponents<MonoBehaviour>())
            if (this != t_behaviour) Destroy(t_behaviour);

        Destroy(GetComponent<NavMeshAgent>());
        RemoveAllForces();
        controller.isFocusSight = false;

        anim.SetBool(FC.AnimatorKey.Aim, false);
        anim.SetBool(FC.AnimatorKey.Crouch, false);
        anim.enabled = false;
        Destroy(weapon.gameObject);
        Destroy(hud.gameObject);
        IsDead = true;
    }

    public override void TakeDamage(Vector3 p_pos, Vector3 p_dir, float p_damage, Collider p_bodyPart = null, GameObject p_hitEffect = null)
    {
        if (!IsDead && isHeadShot && p_bodyPart.transform == anim.GetBoneTransform(HumanBodyBones.Head))
        {
            p_damage *= 10;
            gameController.SendMessage("HeadShotCallback", SendMessageOptions.DontRequireReceiver);
        }

        Instantiate(bloodSample, p_pos, Quaternion.LookRotation(-p_dir), transform);
        health -= p_damage;

        if (!IsDead)
        {
            anim.SetTrigger(FC.AnimatorKey.Hit);
            healthUI.SetVisible();
            UpdateHealthBar();
            controller.variables.isFeelAlert = true;
            controller.personalTarget = controller.aimTarget.position;
        }

        if (health <= 0)
        {
            if (!IsDead) Die();
            Rigidbody t_rigid = p_bodyPart.GetComponent<Rigidbody>();
            t_rigid.mass = 40;
            t_rigid.AddForce(100f * p_dir.normalized, ForceMode.Impulse);
        }
    }
}
