using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorTriggerArea : MonoBehaviour
{
    public DoorEventObject doorEventObject;
    public DoorController doorController;

    public bool autoClose = true;

    private void OnTriggerEnter(Collider other)
    {
        doorEventObject.OpenDoor(doorController.id);
    }

    private void OnTriggerExit(Collider other)
    {
        if (autoClose) doorEventObject.CloseDoor(doorController.id);
    }
}
