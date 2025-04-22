using Sirenix.OdinInspector;
using UnityEngine;

public class GatherResourceHitReaction : AbstractHitReaction
{
    [SerializeField] private ResourceDefinition _resource;
    [SerializeField] private bool _basedOnDamage;
    [SerializeField, ShowIf("_basedOnDamage")] private float _multiplier = 1;
    [SerializeField, HideIf("_basedOnDamage")] private int _gainedPerHit = 1;
    
    public override void OnHit(DamageType damageType, int damageDealt, Vector3 position)
    {
        int resourceGain = _basedOnDamage ? Mathf.CeilToInt(damageDealt * _multiplier) : _gainedPerHit;
        SceneReferences.Instance.ResourceManager.AddResource(_resource, resourceGain);
        UIReferences.Instance.FloatingNumberPanel.DisplayResourceGain(this, _resource, resourceGain, position);
    }
}
