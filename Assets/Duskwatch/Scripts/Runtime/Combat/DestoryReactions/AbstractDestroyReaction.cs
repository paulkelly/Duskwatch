using UnityEngine;

public abstract class AbstractDestroyReaction : MonoBehaviour, IDestroyReaction
{
    public abstract void OnDestroyed();
    public abstract void OnResurrect();
}
