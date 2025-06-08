using System;
using System.Collections.Generic;
using UnityEngine;

public class HarvestResourceTask : AbstractDestroyReaction, IAgentTask
{
    [SerializeField] private Collider _collider;
    [SerializeField] private Health _resourceHealth;
    [SerializeField] private ResourceDefinition _resource;

    public ResourceDefinition Resource => _resource;
    public Vector3 Position { get; private set; }
    private HashSet<DuskwatchAgent> _currentWorkers = new HashSet<DuskwatchAgent>();

    private bool _isActive;
    public bool IsActive
    {
        get => _isActive;
        set
        {
            if(_isActive == value) return;
            _isActive = value;
            if (_isActive)
            {
                SceneReferences.Instance.WorkerTaskManager.RegisterTask(this);
            }
            else
            {
                SceneReferences.Instance.WorkerTaskManager.DeregisterTask(this);
            }
        }
    }

    public bool CanPerformTask(DuskwatchAgent agent)
    {
        WorkerAgent worker = agent as WorkerAgent;
        if(!worker) return false;
        if (worker.HasResource && worker.HeldResource != _resource) return false;
        return _currentWorkers.Count < _resource.maxWorkersPerResource;
    }

    public float GetPriority(DuskwatchAgent worker)
    {
        int workers = SceneReferences.Instance.WorkerTaskManager.GetWorkersCollectingResources(_resource);
        float workerPriority = 1;
        if (workers < _resource.minWorkers.workerCount)
        {
            workerPriority = _resource.minWorkers.priority;
        }
        else if (workers > _resource.maxWorkers.workerCount)
        {
            workerPriority = _resource.maxWorkers.priority;
        }
        else
        {
            workerPriority = Mathf.Lerp(_resource.minWorkers.priority, _resource.maxWorkers.priority, Mathf.InverseLerp(_resource.minWorkers.workerCount, _resource.maxWorkers.workerCount, workers));
        }

        int stockpile = SceneReferences.Instance.ResourceManager.GetResourceAvailability(_resource);
        float resourceStockpilePriority = Mathf.Lerp(1000, 0, Mathf.InverseLerp(500, 2000, stockpile));
        return (workerPriority+resourceStockpilePriority)-Distance(worker.Position);
    }

    public float Distance(Vector3 agentPosition)
    {
        return Vector3.Distance(Position, agentPosition);
    }

    public void StartTask(DuskwatchAgent agent)
    {
        WorkerAgent worker = agent as WorkerAgent;
        SceneReferences.Instance.WorkerTaskManager.AddWorkerCollectingResource(_resource, worker);
        _currentWorkers.Add(worker);
        worker.QueueAction(new MoveAgentAction(worker, () => _collider.ClosestPoint(worker.Position)));
        worker.QueueAction(new HarvestResourceAction(worker, _resourceHealth, _resource));
        worker.QueueAction(new WaitAction(0.3f));
        worker.SetWeapon(_resource.weaponType);
    }

    public void StopTask(DuskwatchAgent worker)
    {
        _currentWorkers.Remove(worker);
        SceneReferences.Instance.WorkerTaskManager.RemoveWorkerCollectingResource(_resource, worker);
    }


    private void OnEnable()
    {
        Position = transform.position;
        SceneReferences.Instance.HarvestTaskManager.RegisterHarvestTask(this);
    }
    
    private void OnDisable()
    {
        if(SceneReferences.Instance == null || SceneReferences.Instance.HarvestTaskManager == null) return;
        SceneReferences.Instance.HarvestTaskManager.DeregisterHarvestTask(this);
    }

    public override void OnDestroyed()
    {
        enabled = false;
    }

    public override void OnResurrect()
    {
        enabled = true;
    }
}
