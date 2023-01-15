using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractable
{
    float Distance { get; }
    bool Interact(GameObject p_otherObj);
    void StopInteract(GameObject p_otherObj);
}
