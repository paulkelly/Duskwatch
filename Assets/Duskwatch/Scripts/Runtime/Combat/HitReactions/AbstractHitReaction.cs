using System;
using UnityEngine;

public abstract class AbstractHitReaction : MonoBehaviour, IHitReaction
{
    [SerializeField] private bool _playerOnly;
    public bool PlayerOnly => _playerOnly;
    public abstract void OnHit(DamageType damageType, int damageDealt, Vector3 position);
}
