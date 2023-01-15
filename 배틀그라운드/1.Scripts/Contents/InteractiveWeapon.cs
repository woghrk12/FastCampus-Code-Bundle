using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InteractiveWeapon : MonoBehaviour
{
    public string labelWeaponName;
    public Sprite weaponSprite;
    
    public SoundList shotSound, reloadSound, pickSound, dropSound, noBulletSound;
    
    public Vector3 rightHandPos;
    public Vector3 relativeRotation;
    
    public float bulletDamage = 10f;
    public float recoilAngle;

    public EWeaponType weaponType = EWeaponType.NONE;
    public EWeaponMode weaponMode = EWeaponMode.SEMI;
    public int burstSize = 1;

    public int curMagCapacity, totalBullets;
    private int fullMag, maxBullets;

    private GameObject player, gameController;
    private ShotBehaviour playerInventory;

    private BoxCollider weaponCollider;
    private SphereCollider interactiveRadius;
    private Rigidbody weaponRigid;
    private bool isPickable;

    public GameObject screenHUD;
    public WeaponUI weaponHUD;
    private Transform pickHUD;
    public Text labelPickHUD;

    public Transform muzzleTransform;

    private void Awake()
    {
        gameObject.name = labelWeaponName;
        gameObject.layer = LayerMask.NameToLayer(FC.TagAndLayer.LayerName.IgnoreRayCast);

        foreach (Transform t_transform in transform)
            t_transform.gameObject.layer = LayerMask.NameToLayer(FC.TagAndLayer.LayerName.IgnoreRayCast);

        player = GameObject.FindGameObjectWithTag(FC.TagAndLayer.TagName.Player);
        playerInventory = player.GetComponent<ShotBehaviour>();
        gameController = GameObject.FindGameObjectWithTag(FC.TagAndLayer.TagName.GameController);

        if (weaponHUD == null)
        {
            if (screenHUD == null)
                screenHUD = GameObject.Find("ScreenHUD");

            weaponHUD = screenHUD.GetComponent<WeaponUI>();
        }
        if (pickHUD == null)
            pickHUD = gameController.transform.Find("PickupHUD");

        weaponCollider = transform.GetChild(0).gameObject.AddComponent<BoxCollider>();
        CreateInteractiveRadius(weaponCollider.center);
        weaponRigid = gameObject.AddComponent<Rigidbody>();

        if (weaponType == EWeaponType.NONE) weaponType = EWeaponType.SHORT;
        
        fullMag = curMagCapacity;
        maxBullets = totalBullets;
        pickHUD.gameObject.SetActive(false);

        if (muzzleTransform == null) muzzleTransform = transform.Find("muzzle");
    }

    private void Update()
    {
        if (isPickable && Input.GetButtonDown(ButtonName.Pick))
        {
            weaponRigid.isKinematic = true;
            weaponCollider.enabled = false;
            playerInventory.AddWeapon(this);
            Destroy(interactiveRadius);
            Toggle(true);
            isPickable = false;

            TogglePickHUD(false);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.gameObject != player && Vector3.Distance(transform.position, player.transform.position) <= 5f)
            SoundManager.Instance.PlayOneShotEffect((int)dropSound, transform.position, 0.5f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == player && playerInventory && playerInventory.isActiveAndEnabled)
        {
            isPickable = true;
            TogglePickHUD(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == player) 
        {
            isPickable = false;
            TogglePickHUD(false);
        }
    }

    private void CreateInteractiveRadius(Vector3 p_center)
    {
        interactiveRadius = gameObject.AddComponent<SphereCollider>();

        interactiveRadius.center = p_center;
        interactiveRadius.radius = 1f;
        interactiveRadius.isTrigger = true;
    }

    private void TogglePickHUD(bool p_isToggle)
    {
        pickHUD.gameObject.SetActive(p_isToggle);
        
        if (p_isToggle)
        {
            pickHUD.position = transform.position + Vector3.up * 0.5f;
            Vector3 t_direction = player.GetComponent<BehaviourController>().camTransform.forward;
            t_direction.y = 0;
            pickHUD.rotation = Quaternion.LookRotation(t_direction);
            labelPickHUD.text = "Pick " + gameObject.name;
        }
    }

    private void UpdateHUD() => weaponHUD.UpdateWeaponHUD(weaponSprite, curMagCapacity, fullMag, totalBullets);

    public void Toggle(bool p_isActive)
    {
        weaponHUD.Toggle(p_isActive);

        if (p_isActive)
        {
            SoundManager.Instance.PlayOneShotEffect((int)pickSound, transform.position, 0.5f);
            UpdateHUD();
        } 
    }

    public void Drop()
    {
        gameObject.SetActive(true);
        transform.position += Vector3.up;
        weaponRigid.isKinematic = false;
        transform.parent = null;
        CreateInteractiveRadius(weaponCollider.center);
        weaponCollider.enabled = true;
        weaponHUD.Toggle(false);
    }

    public bool StartReload()
    {
        if (curMagCapacity == fullMag || totalBullets <= 0) return false;
        else if (totalBullets < fullMag - curMagCapacity)
        {
            curMagCapacity += totalBullets;
            totalBullets = 0;
        }
        else
        {
            totalBullets -= fullMag - curMagCapacity;
            curMagCapacity = fullMag;
        }

        return true;
    }

    public void EndReload() => UpdateHUD();

    public bool Shoot(bool p_isFirstShot = true)
    {
        if (curMagCapacity > 0)
        {
            curMagCapacity--;
            UpdateHUD();
            return true;
        }

        if (p_isFirstShot && noBulletSound != SoundList.None)
            SoundManager.Instance.PlayOneShotEffect((int)noBulletSound, muzzleTransform.position, 5f);

        return false;
    }

    public void ResetBullet()
    {
        curMagCapacity = fullMag;
        totalBullets = maxBullets;
    }
}
