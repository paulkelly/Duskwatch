using PrimeTween;
using UnityEngine;

public class Respawner : AbstractDestroyReaction
{
    [SerializeField] private Health _health;
    [SerializeField] private float _delay;
    
    
    public override void OnDestroyed()
    {
        Tween.Delay(_delay).OnComplete(PerformResurrect);
    }

    public override void OnResurrect()
    {
    }

    private void PerformResurrect()
    {
        _health.Resurrect();
    }

    #if UNITY_EDITOR
    private void Reset()
    {
        _health = GetComponent<Health>();
    }
    #endif
}
