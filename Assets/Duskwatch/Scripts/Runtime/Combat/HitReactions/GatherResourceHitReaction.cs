using Sirenix.OdinInspector;
using UnityEngine;

public class GatherResourceHitReaction : AbstractHitReaction
{
    [SerializeField] private ResourceDefinition _resource;
    [SerializeField] private bool _basedOnDamage;
    [SerializeField, ShowIf("_basedOnDamage")] private float _multiplier = 1;
    [SerializeField, HideIf("_basedOnDamage")] private int _gainedPerHit = 1;

    private float _carried;
    
    public override void OnHit(DamageType damageType, int damageDealt, Vector3 position)
    {
        float multipliedDamage = (damageDealt * _multiplier) + _carried;
        int resourceGain = _basedOnDamage ? Mathf.FloorToInt(multipliedDamage) : _gainedPerHit;
        if (_basedOnDamage)
        {
            _carried = multipliedDamage - resourceGain;
        }
        
        SceneReferences.Instance.ResourceManager.AddResource(_resource, resourceGain);
        UIReferences.Instance.FloatingNumberPanel.DisplayResourceGain(this, _resource, resourceGain, position);
    }
}
