using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DamageText : MonoBehaviour
{
    public float delayTimeToDestroy = 1.0f;
    public int Damage;
    public TextMesh textMesh;

    private void Start()
    {
        textMesh.text = Damage.ToString();
        Destroy(gameObject, delayTimeToDestroy);
    }
}
