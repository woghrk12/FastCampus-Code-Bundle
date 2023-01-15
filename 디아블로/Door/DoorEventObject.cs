using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Event System", menuName = "Event System/Door Event Object")]
public class DoorEventObject : ScriptableObject
{
    [NonSerialized] public Action<int> OnOpenDoor;
    [NonSerialized] public Action<int> OnCloseDoor;

    public void OpenDoor(int p_id) { OnOpenDoor?.Invoke(p_id); }

    public void CloseDoor(int p_id) { OnCloseDoor?.Invoke(p_id); }
}
