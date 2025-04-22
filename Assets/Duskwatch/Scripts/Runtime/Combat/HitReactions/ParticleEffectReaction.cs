using UnityEngine;

public class ParticleEffectReaction : AbstractHitReaction
{
    [SerializeField] private ParticleSystem _effect;
    [SerializeField] private bool _atHitPosition;
    public override void OnHit(DamageType damageType, int damageDealt, Vector3 position)
    {
        if (_atHitPosition)
        {
            transform.position = position;
        }
        _effect.Play();
    }
}
