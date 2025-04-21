using UnityEngine;

public abstract class AbstractDestroyReaction : MonoBehaviour, IDestroyReaction
{
    public abstract void OnDestroyed();
    public abstract void OnResurrect();
    
    private void Reset()
    {
        var healthComponent = GetComponent<Health>();
        if(healthComponent == null) return;
        
        healthComponent.RegisterDestroyReaction(this);
    }
}
