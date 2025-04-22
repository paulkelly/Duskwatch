using AudioSystem;
using UnityEngine;

public class SFXHitReaction : AbstractHitReaction
{
    public SoundData _sfx;
    public override void OnHit(DamageType damageType, int damageDealt, Vector3 position)
    {
        _sfx.Play(position);
    }
}
