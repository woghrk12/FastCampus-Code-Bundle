using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DelayedDestroy : MonoBehaviour
{
    public float delayTime = 0.5f;

    private void Start() => Destroy(gameObject, delayTime);
}
