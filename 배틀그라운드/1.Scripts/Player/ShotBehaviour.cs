using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotBehaviour : GenericBehaviour
{
    public Texture2D aimCrossHair, shotCrossHair;
    public GameObject muzzleFlashEffect, shotEffect, sparkEffect;
    public Material bulletHoleMesh;
    public int maxBulletHoles = 50;
    private List<GameObject> bulletHoles;
    private int bulletHoleSlot = 0;
    private int numBurstShot = 0;

    public float shotErrorRate = 0.01f;
    public float shotRateFactor = 1f;

    public float armsRotation = 8f;
    public LayerMask shotMask = ~(FC.TagAndLayer.LayerMasking.IgnoreRayCast 
        | FC.TagAndLayer.LayerMasking.IgnoreShot
        | FC.TagAndLayer.LayerMasking.CoverInvisible 
        | FC.TagAndLayer.LayerMasking.Player);
    public LayerMask organicMask = FC.TagAndLayer.LayerMasking.Player | FC.TagAndLayer.LayerMasking.Enemy;

    public Vector3 leftArmShortAim = new Vector3(-4.0f, 0.0f, 2.0f);

    private int activeWeapon = 0;

    private int weaponTypeHash;
    private int changeWeaponTriggerHash;
    private int shotTriggerHash;
    private int isAimHash, isBlockedAimHash, isReloadingHash;

    private List<InteractiveWeapon> weapons;
    private Dictionary<EWeaponType, int> slotMap;

    private bool isAiming, isAimBlocked;

    private Transform muzzlePosition;
    private float distToHand;

    private Vector3 castRelativeOrigin;

    private Transform hips, spine, chest, rightHand, leftArm;
    private Vector3 initialRootRotation, initialHipsRotation, initialSpineRotation, initialChestRotation;

    private float shotInterval, originalShotInterval = 0.5f;

    private AimBehaviour aimBehaviour;
    private Texture2D originalCrossHair;

    private bool isShooting = false, isChangingWeapon = false, isShotAlive = false;

    private void Start()
    {
        weaponTypeHash = Animator.StringToHash(FC.AnimatorKey.Weapon);
        isAimHash = Animator.StringToHash(FC.AnimatorKey.Aim);
        isBlockedAimHash = Animator.StringToHash(FC.AnimatorKey.BlockedAim);
        changeWeaponTriggerHash = Animator.StringToHash(FC.AnimatorKey.ChangeWeapon);
        shotTriggerHash = Animator.StringToHash(FC.AnimatorKey.Shooting);
        isReloadingHash = Animator.StringToHash(FC.AnimatorKey.Reload);

        weapons = new List<InteractiveWeapon>(new InteractiveWeapon[3]);
        bulletHoles = new List<GameObject>();

        aimBehaviour = GetComponent<AimBehaviour>();

        muzzleFlashEffect.SetActive(false);
        shotEffect.SetActive(false);
        sparkEffect.SetActive(false);

        slotMap = new Dictionary<EWeaponType, int>
        {
            { EWeaponType.SHORT, 1},
            { EWeaponType.LONG, 2}
        };

        Transform t_neckTransform = controller.Anim.GetBoneTransform(HumanBodyBones.Neck);
        if (!t_neckTransform) t_neckTransform = controller.Anim.GetBoneTransform(HumanBodyBones.Head).parent;
        hips = controller.Anim.GetBoneTransform(HumanBodyBones.Hips);
        spine = controller.Anim.GetBoneTransform(HumanBodyBones.Spine);
        chest = controller.Anim.GetBoneTransform(HumanBodyBones.Chest);
        rightHand = controller.Anim.GetBoneTransform(HumanBodyBones.RightHand);
        leftArm = controller.Anim.GetBoneTransform(HumanBodyBones.LeftUpperArm);

        initialRootRotation = (hips.parent == transform) ? Vector3.zero : hips.parent.localEulerAngles;
        initialHipsRotation = hips.localEulerAngles;
        initialSpineRotation = spine.localEulerAngles;
        initialChestRotation = chest.localEulerAngles;
        
        originalCrossHair = aimBehaviour.crossHair;

        shotInterval = originalShotInterval;

        castRelativeOrigin = t_neckTransform.position - transform.position;
        distToHand = (rightHand.position - t_neckTransform.position).magnitude * 1.5f;
    }

    private void Update()
    {
        float t_shotTrigger = Mathf.Abs(Input.GetAxisRaw(ButtonName.Shoot));
        if (t_shotTrigger > Mathf.Epsilon && !isShooting && activeWeapon > 0 && numBurstShot == 0)
        {
            isShooting = true;
            UseWeapon(activeWeapon);
        }
        else if (isShooting && t_shotTrigger < Mathf.Epsilon)
            isShooting = false;
        else if (Input.GetButtonUp(ButtonName.Reload) && activeWeapon > 0)
        {
            if (weapons[activeWeapon].StartReload())
            {
                SoundManager.Instance.PlayOneShotEffect((int)weapons[activeWeapon].reloadSound, muzzlePosition.position, 0.5f);
                controller.Anim.SetBool(isReloadingHash, true);
            }
        }
        else if (Input.GetButtonDown(ButtonName.Drop) && activeWeapon > 0)
        {
            EndReloadWeapon();
            int t_weaponToDrop = activeWeapon;
            ChangeWeapon(activeWeapon, 0);
            weapons[t_weaponToDrop].Drop();
            weapons[t_weaponToDrop] = null;
        }
        else
        {
            if (Mathf.Abs(Input.GetAxisRaw(ButtonName.Change)) > Mathf.Epsilon && !isChangingWeapon)
            {
                isChangingWeapon = true;
                ChangeWeapon(activeWeapon, (activeWeapon + 1) % weapons.Count);
            }
            else if (Mathf.Abs(Input.GetAxisRaw(ButtonName.Change)) < Mathf.Epsilon) isChangingWeapon = false;
        }

        if (isShotAlive) Shot();

        isAiming = controller.Anim.GetBool(isAimHash);
    }

    private void LateUpdate()
    {
        if (isAiming && weapons[activeWeapon] && weapons[activeWeapon].weaponType == EWeaponType.SHORT)
            leftArm.localEulerAngles = leftArm.localEulerAngles + leftArmShortAim;
    }

    private void OnAnimatorIK(int layerIndex)
    {
        if (isAiming && activeWeapon > 0)
        {
            if (CheckForBlockedAim()) return;

            Quaternion t_targetRotation = Quaternion.Euler(0, transform.eulerAngles.y, 0);
            t_targetRotation *= Quaternion.Euler(initialRootRotation);
            t_targetRotation *= Quaternion.Euler(initialHipsRotation);
            t_targetRotation *= Quaternion.Euler(initialSpineRotation);

            controller.Anim.SetBoneLocalRotation(HumanBodyBones.Spine, Quaternion.Inverse(hips.rotation) * t_targetRotation);

            float t_xCamRotation = Quaternion.LookRotation(controller.camTransform.forward).eulerAngles.x;
            t_targetRotation = Quaternion.AngleAxis(t_xCamRotation + armsRotation, transform.right);

            if (weapons[activeWeapon] && weapons[activeWeapon].weaponType == EWeaponType.LONG)
            {
                t_targetRotation *= Quaternion.AngleAxis(9f, transform.right);
                t_targetRotation *= Quaternion.AngleAxis(20f, transform.up);
            }

            t_targetRotation *= spine.rotation;
            t_targetRotation *= Quaternion.Euler(initialChestRotation);
            controller.Anim.SetBoneLocalRotation(HumanBodyBones.Chest, Quaternion.Inverse(spine.rotation) * t_targetRotation);
        }
    }

    private void DrawShoot(GameObject p_weapon, Vector3 p_dest, Vector3 p_targetNormal, Transform p_parent, bool p_isSpark = true, bool p_isHole = true)
    {
        Vector3 t_origin = muzzlePosition.position - muzzlePosition.right * 0.5f;

        muzzleFlashEffect.SetActive(true);
        muzzleFlashEffect.transform.SetParent(muzzlePosition);
        muzzleFlashEffect.transform.localPosition = Vector3.zero;
        muzzleFlashEffect.transform.localEulerAngles = Vector3.back * 90f;

        GameObject t_shotEffect = EffectManager.Instance.EffectOneShot((int)EffectList.tracer5, t_origin);
        t_shotEffect.SetActive(true);
        t_shotEffect.transform.rotation = Quaternion.LookRotation(p_dest - t_origin);
        t_shotEffect.transform.parent = shotEffect.transform.parent;

        if (p_isSpark) 
        {
            GameObject t_sparkEffect = EffectManager.Instance.EffectOneShot((int)EffectList.sparks4, p_dest);
            t_sparkEffect.SetActive(true);
            t_sparkEffect.transform.parent = sparkEffect.transform.parent;
        }

        if (p_isHole)
        {
            Quaternion t_hitRotation = Quaternion.FromToRotation(Vector3.back, p_targetNormal);
            GameObject t_bulletHole;
            if (bulletHoles.Count < maxBulletHoles)
            {
                t_bulletHole = GameObject.CreatePrimitive(PrimitiveType.Quad);
                t_bulletHole.GetComponent<MeshRenderer>().material = bulletHoleMesh;
                t_bulletHole.GetComponent<Collider>().enabled = false;
                t_bulletHole.transform.localScale = Vector3.one * 0.07f;
                t_bulletHole.name = "BulletHole";
                bulletHoles.Add(t_bulletHole);
            }
            else
            {
                t_bulletHole = bulletHoles[bulletHoleSlot];
                bulletHoleSlot++;
                bulletHoleSlot %= maxBulletHoles;
            }
            t_bulletHole.transform.position = p_dest + 0.01f * p_targetNormal;
            t_bulletHole.transform.rotation = t_hitRotation;
            t_bulletHole.transform.SetParent(p_parent);
        }
    }

    private void UseWeapon(int p_weapon, bool p_isFirstShot = true)
    {
        if (!isAiming || isAimBlocked || controller.Anim.GetBool(isReloadingHash) || !weapons[p_weapon].Shoot(p_isFirstShot)) return;

        numBurstShot++;
        controller.Anim.SetTrigger(shotTriggerHash);
        aimBehaviour.crossHair = shotCrossHair;
        controller.CamScript.BounceVertical(weapons[p_weapon].recoilAngle);

        Vector3 t_imprecision = Random.Range(-shotErrorRate, shotErrorRate) * controller.camTransform.forward;
        Ray t_ray = new Ray(controller.camTransform.position, controller.camTransform.forward + t_imprecision);
        if (Physics.Raycast(t_ray, out RaycastHit t_hitInfo, 500f, shotMask))
        {
            if (t_hitInfo.collider.transform != transform)
            {
                bool t_isOrganic = (organicMask == (organicMask | 1 << t_hitInfo.transform.root.gameObject.layer));
                DrawShoot(weapons[p_weapon].gameObject, t_hitInfo.point, t_hitInfo.normal, t_hitInfo.collider.transform, !t_isOrganic, !t_isOrganic);
                if (t_hitInfo.collider)
                {
                    t_hitInfo.collider.SendMessageUpwards("OnDamage", new DamageInfo(t_hitInfo.point,
                        t_ray.direction,
                        weapons[p_weapon].bulletDamage,
                        t_hitInfo.collider), SendMessageOptions.DontRequireReceiver);
                }
            }
        }
        else
        {
            Vector3 t_dest = (t_ray.direction * 500f) - t_ray.origin;
            DrawShoot(weapons[p_weapon].gameObject, t_dest, Vector3.up, null, false, false);
        }

        SoundManager.Instance.PlayOneShotEffect((int)weapons[p_weapon].shotSound, muzzlePosition.position, 5f);
        GameObject.FindGameObjectWithTag(FC.TagAndLayer.TagName.GameController).SendMessage("RootAlertNearBy", t_ray.origin, SendMessageOptions.DontRequireReceiver);
        shotInterval = originalShotInterval;
        isShotAlive = true;
    }

    public void EndReloadWeapon()
    {
        controller.Anim.SetBool(isReloadingHash, false);
        weapons[activeWeapon].EndReload();
    }

    private void SetWeaponCrossHair(bool p_isArmed)
    {
        aimBehaviour.crossHair = p_isArmed ? aimCrossHair : originalCrossHair;
    }

    private void Shot()
    {
        if (shotInterval > 0.2f)
        {
            shotInterval -= shotRateFactor * Time.deltaTime;
            if (shotInterval <= 0.4f)
            {
                SetWeaponCrossHair(activeWeapon > 0);
                muzzleFlashEffect.SetActive(false);
                if (activeWeapon > 0)
                {
                    controller.CamScript.BounceVertical(-weapons[activeWeapon].recoilAngle * 0.1f);
                    if (shotInterval <= (0.4f - 2f * Time.deltaTime))
                    {
                        if (weapons[activeWeapon].weaponMode == EWeaponMode.AUTO && Input.GetAxisRaw(ButtonName.Shoot) != 0)
                            UseWeapon(activeWeapon, false);
                        else if (weapons[activeWeapon].weaponMode == EWeaponMode.BURST && numBurstShot < weapons[activeWeapon].burstSize)
                            UseWeapon(activeWeapon, false);
                        else if (weapons[activeWeapon].weaponMode != EWeaponMode.BURST)
                            numBurstShot = 0;
                    }
                }
            }
        }
        else
        {
            isShotAlive = false;
            controller.CamScript.BounceVertical(0);
            numBurstShot = 0;
        }
    }

    private void ChangeWeapon(int p_oldWeapon, int p_newWeapon)
    {
        if (p_oldWeapon > 0)
        {
            weapons[p_oldWeapon].gameObject.SetActive(false);
            muzzlePosition = null;
            weapons[p_oldWeapon].Toggle(false);
        }

        while (weapons[p_newWeapon] == null && p_newWeapon > 0)
        {
            p_newWeapon = (p_newWeapon + 1) % weapons.Count;
        }

        if (p_newWeapon > 0)
        {
            weapons[p_newWeapon].gameObject.SetActive(true);
            muzzlePosition = weapons[p_newWeapon].transform.Find("Muzzle");
            weapons[p_newWeapon].Toggle(true);
        }

        activeWeapon = p_newWeapon;
        if (p_oldWeapon != p_newWeapon)
        {
            controller.Anim.SetTrigger(changeWeaponTriggerHash);
            controller.Anim.SetInteger(weaponTypeHash, weapons[p_newWeapon] ? (int)weapons[p_newWeapon].weaponType : 0);
        }

        SetWeaponCrossHair(p_newWeapon > 0);
    }

    public void AddWeapon(InteractiveWeapon p_weapon)
    {
        p_weapon.gameObject.transform.SetParent(rightHand);
        p_weapon.transform.localPosition = p_weapon.rightHandPos;
        p_weapon.transform.localRotation = Quaternion.Euler(p_weapon.relativeRotation);

        if (weapons[slotMap[p_weapon.weaponType]])
        {
            if (weapons[slotMap[p_weapon.weaponType]].labelWeaponName == p_weapon.labelWeaponName)
            {
                weapons[slotMap[p_weapon.weaponType]].ResetBullet();
                ChangeWeapon(activeWeapon, slotMap[p_weapon.weaponType]);
                Destroy(p_weapon.gameObject);
                return;
            }
            else
                weapons[slotMap[p_weapon.weaponType]].Drop();
        }

        weapons[slotMap[p_weapon.weaponType]] = p_weapon;
        ChangeWeapon(activeWeapon, slotMap[p_weapon.weaponType]);
    }

    private bool CheckForBlockedAim()
    {
        isAimBlocked = Physics.SphereCast(transform.position + castRelativeOrigin, 
            0.1f,
            controller.CamScript.transform.forward,
            out RaycastHit t_hitInfo,
            distToHand - 0.1f);

        isAimBlocked = isAimBlocked && t_hitInfo.collider.transform != transform;

        Debug.DrawRay(transform.position + castRelativeOrigin, controller.camTransform.transform.forward * distToHand, isAimBlocked ? Color.red : Color.cyan);
        
        return isAimBlocked;
    }
}
