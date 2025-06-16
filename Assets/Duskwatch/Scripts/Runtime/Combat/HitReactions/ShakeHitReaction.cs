using System;
using PrimeTween;
using UnityEngine;

public class ShakeHitReaction : AbstractHitReaction
{
    [SerializeField] private ShakeSettings _shakeSettings;

    public override void OnHit(DamageType damageType, int damageDealt, Vector3 position, bool wasPlayer)
    {
        Tween.ShakeLocalRotation(transform, _shakeSettings);
    }
}
