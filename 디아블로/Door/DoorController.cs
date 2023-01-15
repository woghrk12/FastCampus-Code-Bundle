using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorController : MonoBehaviour
{
    public DoorEventObject doorEventObject;

    public int id = 0;
    public float openOffset = 4f;
    public float closeOffset = 1f;

    private void OnEnable()
    {
        doorEventObject.OnOpenDoor += OnOpenDoor;
        doorEventObject.OnCloseDoor += OnCloseDoor;
    }

    private void OnDisable()
    {
        doorEventObject.OnOpenDoor -= OnOpenDoor;
        doorEventObject.OnCloseDoor -= OnCloseDoor;
    }

    public void OnOpenDoor(int p_id)
    {
        if (p_id != id) return;
        StopAllCoroutines();
        StartCoroutine(OpenDoor());
    }

    public void OnCloseDoor(int p_id)
    {
        if (p_id != id) return;
        StopAllCoroutines();
        StartCoroutine(CloseDoor());
    }

    private IEnumerator OpenDoor()
    {
        while (transform.position.y < openOffset)
        {
            Vector3 t_calcPos = transform.position;
            t_calcPos.y += 0.01f;
            transform.position = t_calcPos;
            yield return null;
        }
    }

    private IEnumerator CloseDoor()
    {
        while (transform.position.y > closeOffset)
        {
            Vector3 t_calcPos = transform.position;
            t_calcPos.y -= 0.01f;
            transform.position = t_calcPos;
            yield return null;
        }
    }
}
