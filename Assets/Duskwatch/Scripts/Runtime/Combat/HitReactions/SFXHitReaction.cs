using AudioSystem;
using UnityEngine;

public class SFXHitReaction : AbstractHitReaction
{
    public SoundData _sfx;
    public float nonPlayerVolume = 1;
    public override void OnHit(DamageType damageType, int damageDealt, Vector3 position, bool wasPlayer)
    {
        _sfx.Play(position, wasPlayer ? 1 : nonPlayerVolume);
    }
}
