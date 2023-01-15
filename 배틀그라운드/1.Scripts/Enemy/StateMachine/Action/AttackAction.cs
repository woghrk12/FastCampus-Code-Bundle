using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Action Action", menuName = "Pluggable AI/Actions/Action")]
public class AttackAction : Action
{
    private readonly float startShotDelay = 0.2f;
    private readonly float aimAngleGap = 30f;

    public override void OnReadyAction(StateController p_controller)
    {
        p_controller.variables.shotsInRounds = Random.Range(p_controller.maxBurst / 2, p_controller.maxBurst);
        p_controller.variables.curShots = 0;
        p_controller.variables.startShotTimer = 0f;
        p_controller.enemyAnim.anim.ResetTrigger(FC.AnimatorKey.Shooting);
        p_controller.enemyAnim.anim.SetBool(FC.AnimatorKey.Crouch, false);
        p_controller.variables.waitInCoverTime = 0f;
        p_controller.enemyAnim.ActivatePendingAim();
    }

    public override void Act(StateController p_controller)
    {
        p_controller.isFocusSight = true;

        if (CanShot(p_controller)) Shot(p_controller);

        p_controller.variables.blindEngageTimer += Time.deltaTime;
    }

    private void DoShot(StateController p_controller, Vector3 p_dir, Vector3 p_hitPoint, Vector3 p_hitNormal = default, bool p_isOrganic = false, Transform p_target = null)
    {
        GameObject t_muzzleFlash = EffectManager.Instance.EffectOneShot((int)EffectList.flash1, Vector3.zero);
        t_muzzleFlash.transform.SetParent(p_controller.enemyAnim.muzzleTr);
        t_muzzleFlash.transform.localPosition = Vector3.zero;
        t_muzzleFlash.transform.localEulerAngles = Vector3.left * 90f;
        
        DelayedDestroy t_delayedDestroy = t_muzzleFlash.AddComponent<DelayedDestroy>();
        t_delayedDestroy.delayTime = 0.5f;

        GameObject t_shotTracer = EffectManager.Instance.EffectOneShot((int)EffectList.tracer5, Vector3.zero);
        Vector3 t_origin = p_controller.enemyAnim.muzzleTr.position;
        t_shotTracer.transform.SetParent(p_controller.enemyAnim.muzzleTr);
        t_shotTracer.transform.position = t_origin;
        t_shotTracer.transform.rotation = Quaternion.LookRotation(p_dir);

        if (p_target && !p_isOrganic)
        {
            GameObject t_bulletHole = EffectManager.Instance.EffectOneShot((int)EffectList.bulletHole0, p_hitPoint + 0.01f * p_hitNormal);
            t_bulletHole.transform.rotation = Quaternion.FromToRotation(Vector3.up, p_hitNormal);

            GameObject t_spark = EffectManager.Instance.EffectOneShot((int)EffectList.sparks4, p_hitPoint);
        }
        else if (p_target && p_isOrganic)
        {
            if (p_target.TryGetComponent(out HealthBase t_targetHealth))
                t_targetHealth.TakeDamage(p_hitPoint, p_dir, p_controller.classStats.BulletDamage, p_target.GetComponent<Collider>(), p_controller.gameObject);
        }

        SoundManager.Instance.PlayShotSound(p_controller.classID, p_controller.enemyAnim.muzzleTr.position, 2f);
    }

    private void CastShot(StateController p_controller)
    {
        Vector3 t_imprecision = Random.Range(-p_controller.classStats.ShotErrorRate, p_controller.classStats.ShotErrorRate) * p_controller.transform.right;
        t_imprecision += Random.Range(-p_controller.classStats.ShotErrorRate, p_controller.classStats.ShotErrorRate) * p_controller.transform.up;

        Vector3 t_shotDir = p_controller.personalTarget - p_controller.enemyAnim.muzzleTr.position;
        t_shotDir = t_shotDir.normalized + t_imprecision;

        Ray t_ray = new Ray(p_controller.enemyAnim.muzzleTr.position, t_shotDir);
        if (Physics.Raycast(t_ray, out RaycastHit t_hitInfo, p_controller.viewRadius, p_controller.generalStats.shotMask.value))
        {
            bool t_isOrganic = ((1 << t_hitInfo.transform.root.gameObject.layer) & p_controller.generalStats.targetMask) != 0;
            DoShot(p_controller, t_ray.direction, t_hitInfo.point, t_hitInfo.normal, t_isOrganic, t_hitInfo.transform);
        }
        else
            DoShot(p_controller, t_ray.direction, t_ray.origin + (t_ray.direction * 500f));
    }

    private bool CanShot(StateController p_controller)
    {
        float t_dist = (p_controller.personalTarget - p_controller.enemyAnim.muzzleTr.position).sqrMagnitude;
        
        if (p_controller.IsAiming && (p_controller.enemyAnim.curAimingAngleGap < aimAngleGap || t_dist <= 5.0f))
            if (p_controller.variables.startShotTimer >= startShotDelay) return true;
            else p_controller.variables.startShotTimer += Time.deltaTime;

        return false;
    }

    private void Shot(StateController p_controller)
    {
        if (Time.timeScale > 0 && p_controller.variables.shotTimer == 0f)
        {
            p_controller.enemyAnim.anim.SetTrigger(FC.AnimatorKey.Shooting);
            CastShot(p_controller);
        }
        else if (p_controller.variables.shotTimer >= (0.1f * 2f * Time.deltaTime))
        {
            p_controller.bullets = Mathf.Max(--p_controller.bullets, 0);
            p_controller.variables.curShots++;
            p_controller.variables.shotTimer = 0;
            return;
        }

        p_controller.variables.shotTimer += p_controller.classStats.ShotRateFactor * Time.deltaTime;
    }
}
