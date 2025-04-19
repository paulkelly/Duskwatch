using System;
using UnityEngine;

public abstract class InteractableObj : MonoBehaviour, IInteractable
{
    public virtual float InteractTime => 2f;
    public float HeldTime { get; set; }
    public Vector3 Position { get; private set; }
    public abstract void Interact();

    private void OnEnable()
    {
        Position = transform.position;
    }
}
