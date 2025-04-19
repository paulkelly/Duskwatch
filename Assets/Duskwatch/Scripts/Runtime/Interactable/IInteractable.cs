using UnityEngine;

public interface IInteractable
{
    public float InteractTime { get; }
    public float HeldTime { get; set; }
    public Vector3 Position { get; }
    public void Interact();
}
