using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HurtHUD : MonoBehaviour
{
    struct HurtData
    {
        public Transform shotOrigin;
        public Image hurtImage;
    }

    private Transform canvas;
    private GameObject hurtPrefab;
    private float decayFactor = 0.8f;
    private Dictionary<int, HurtData> hurtUIData;
    private Transform player, mainCam;

    public void SetUp(Transform p_canvas, GameObject p_hurtPrefab, float p_decayFactor, Transform p_player)
    {
        hurtUIData = new Dictionary<int, HurtData>();
        canvas = p_canvas;
        hurtPrefab = p_hurtPrefab;
        decayFactor = p_decayFactor;
        mainCam = Camera.main.transform;
    }

    private void Update()
    {
        List<int> t_toRemoveKeys = new List<int>();
        foreach (int t_key in hurtUIData.Keys)
        {
            SetRotation(hurtUIData[t_key].hurtImage, mainCam.forward, hurtUIData[t_key].shotOrigin.position - player.position);
            hurtUIData[t_key].hurtImage.color = GetUpdatedAlpha(hurtUIData[t_key].hurtImage.color);
            if (hurtUIData[t_key].hurtImage.color.a <= 0f) t_toRemoveKeys.Add(t_key);
        }

        for (int i = 0; i < t_toRemoveKeys.Count; i++)
        {
            Destroy(hurtUIData[t_toRemoveKeys[i]].hurtImage.transform.gameObject);
            hurtUIData.Remove(t_toRemoveKeys[i]);
        }
    }

    private void SetRotation(Image p_hurtUI, Vector3 p_orientation, Vector3 p_shotDir)
    {
        p_orientation.y = 0;
        p_shotDir.y = 0;
        
        float t_rotation = Vector3.SignedAngle(p_shotDir, p_orientation, Vector3.up);
        Vector3 t_newRotation = p_hurtUI.rectTransform.rotation.eulerAngles;
        t_newRotation.z = t_rotation;

        Image t_hurtImage = p_hurtUI.GetComponent<Image>();
        t_hurtImage.rectTransform.rotation = Quaternion.Euler(t_newRotation);
    }

    private Color GetUpdatedAlpha(Color p_curColor, bool isReset = false)
    {
        if (isReset) p_curColor.a = 1f;
        else p_curColor.a -= decayFactor * Time.deltaTime;

        return p_curColor;
    }

    public void DrawHurtUI(Transform p_shotOrigin, int p_hashID)
    {
        if (hurtUIData.ContainsKey(p_hashID))
            hurtUIData[p_hashID].hurtImage.color = GetUpdatedAlpha(hurtUIData[p_hashID].hurtImage.color, true);
        else
        {
            GameObject t_hurtUI = Instantiate(hurtPrefab, canvas);
            SetRotation(t_hurtUI.GetComponent<Image>(), mainCam.forward, p_shotOrigin.position - player.position);
            
            HurtData t_data;
            t_data.shotOrigin = p_shotOrigin;
            t_data.hurtImage = t_hurtUI.GetComponent<Image>();
            hurtUIData.Add(p_hashID, t_data);
        }
    }
}
