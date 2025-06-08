using System;
using UnityEngine;

public interface IHitReaction
{
    public void OnHit(DamageType damageType, int damageDealt, Vector3 position);
    public bool PlayerOnly { get; }
}

public interface IDestroyReaction
{
    public void OnDestroyed();
    public void OnResurrect();
}