using UnityEngine;

public interface IInteractable
{
    public float InteractTime { get; }
    public float Progress { get; }
    public bool Interacting { get; set; }
    public bool IsClosest { get; set; }
    public Vector3 Position { get; }
    public void Interact();
}
