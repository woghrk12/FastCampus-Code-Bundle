using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponUI : MonoBehaviour
{
    public Color bulletColor = Color.white;
    public Color emptyBulletColor = Color.black;
    private Color noBulletColor;

    [SerializeField] private Image weaponHUD;
    [SerializeField] private GameObject bulletMag;
    [SerializeField] private Text totalBulletsHUD;

    private void Start()
    {
        noBulletColor = new Color(0f, 0f, 0f, 0f);

        if (weaponHUD == null)
            weaponHUD = transform.Find("WeaponHUD/Weapon").GetComponent<Image>();
        if (bulletMag == null)
            bulletMag = transform.Find("WeaponHUD/Data/Mag").gameObject;
        if (totalBulletsHUD == null)
            totalBulletsHUD = transform.Find("WeaponHUD/Data/Label").GetComponent<Text>();

        Toggle(false);
    }

    public void Toggle(bool p_isActive)
    {
        weaponHUD.transform.parent.gameObject.SetActive(p_isActive);
    }

    public void UpdateWeaponHUD(Sprite p_weaponSprite, int p_bulletLeft, int p_fullMag, int p_extraBullets)
    {
        if (p_weaponSprite != null && weaponHUD.sprite != p_weaponSprite)
        {
            weaponHUD.sprite = p_weaponSprite;
            weaponHUD.type = Image.Type.Filled;
            weaponHUD.fillMethod = Image.FillMethod.Horizontal;
        }

        int t_numBullet = 0;
        foreach (Transform t_bulletTransform in bulletMag.transform)
        {
            if (t_numBullet < p_bulletLeft)
                t_bulletTransform.GetComponent<Image>().color = bulletColor;
            else if (t_numBullet >= p_fullMag)
                t_bulletTransform.GetComponent<Image>().color = noBulletColor;
            else
                t_bulletTransform.GetComponent<Image>().color = emptyBulletColor;

            t_numBullet++;
        }

        totalBulletsHUD.text = p_bulletLeft + " / " + p_extraBullets;
    }
}
