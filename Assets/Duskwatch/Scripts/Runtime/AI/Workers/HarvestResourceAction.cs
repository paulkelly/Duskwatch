using UnityEngine;

public class HarvestResourceAction : IAgentAction
{
    private WorkerAgent _worker;
    private Health _resourceHealth;
    private Transform _resourceTransform;
    private ResourceDefinition _resource;
    
    
    public HarvestResourceAction(WorkerAgent worker, Health resourceHealth, ResourceDefinition resource)
    {
        _worker = worker;
        _resourceHealth = resourceHealth;
        _resourceTransform = resourceHealth.transform;
        _resource = resource;
    }

    public bool Complete => (_worker.HasResource && _worker.HoldingMaxAmount) || !_resourceHealth.alive;
    public void Start()
    {
        _worker.Harvesting = true;
        _worker.RotationTarget = _resourceTransform;
    }

    public void Stop()
    {
        _worker.Harvesting = false;
        _worker.RotationTarget = null;
        
        if(!_worker.HasResource) return;
        if(!_worker.HoldingMaxAmount) return;
        ReturnResourceComponent closestReturnBuilding = SceneReferences.Instance.BuildingManager.GetClosestReturnBuilding(_worker.Position, _resource);
        if (closestReturnBuilding != null)
        {
            _worker.QueueAction(new MoveAgentAction(_worker, () => closestReturnBuilding.Collider.ClosestPoint(_worker.Position)));
            _worker.QueueAction(new ReturnResourceAction(_worker));
        }
    }

    public void OnAgentHit()
    {
        if (_resourceHealth.alive)
        {
            _resourceHealth.Damage(DamageType.Melee, _resource.damagePerHit, false, _worker.Position);
            _worker.HarvestResource(_resource);
        }
    }
}
