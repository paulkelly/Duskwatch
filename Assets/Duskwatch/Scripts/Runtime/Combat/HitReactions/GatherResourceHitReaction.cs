using UnityEngine;

public class GatherResourceHitReaction : AbstractHitReaction
{
    [SerializeField] private ResourceDefinition _resource;
    
    public override void OnHit(DamageType damageType, int damageDealt, Vector3 position)
    {
        SceneReferences.Instance.ResourceManager.AddResource(_resource, damageDealt);
        UIReferences.Instance.FloatingNumberPanel.DisplayResourceGain(this, _resource, damageDealt, position);
    }
}
