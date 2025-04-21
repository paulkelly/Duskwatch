using UnityEngine;

public class ParticleEffectReaction : AbstractHitReaction
{
    [SerializeField] private ParticleSystem _effect;
    public override void OnHit(DamageType damageType, int damageDealt, Vector3 position)
    {
        _effect.Play();
    }
}
