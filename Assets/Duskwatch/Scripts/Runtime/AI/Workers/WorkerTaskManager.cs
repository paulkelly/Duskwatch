using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Profiling;

public class WorkerTaskManager : MonoBehaviour
{
    public HashSet<IAgentTask> _availableTasks = new HashSet<IAgentTask>();
    public Dictionary<ResourceDefinition, HashSet<DuskwatchAgent>> _workersCollectingResource = new Dictionary<ResourceDefinition, HashSet<DuskwatchAgent>>();

    public IAgentTask GetTask(WorkerAgent agent)
    {
        Profiler.BeginSample("Get Worker Task");
        RegisterTask(new TestTask(transform));
        var task = _availableTasks.Where(t => t.CanPerformTask(agent)).OrderByDescending(t => t.GetPriority(agent)).First();
        Profiler.EndSample();
        return task;
    }
    public void RegisterTask(IAgentTask task)
    {
        _availableTasks.Add(task);
    }

    public void DeregisterTask(IAgentTask task)
    {
        _availableTasks.Remove(task);
    }

    public int GetWorkersCollectingResources(ResourceDefinition resource)
    {
        if (_workersCollectingResource.ContainsKey(resource))
        {
            return _workersCollectingResource[resource].Count;
        }

        return 0;
    }

    public void AddWorkerCollectingResource(ResourceDefinition resource, DuskwatchAgent worker)
    {
        if (!_workersCollectingResource.ContainsKey(resource))
        {
            _workersCollectingResource.Add(resource, new HashSet<DuskwatchAgent>());
        }

        _workersCollectingResource[resource].Add(worker);
    }
    
    public void RemoveWorkerCollectingResource(ResourceDefinition resource, DuskwatchAgent worker)
    {
        if (!_workersCollectingResource.ContainsKey(resource))
        {
            return;
        }

        _workersCollectingResource[resource].Remove(worker);
    }
    
    
}
