using System;
using System.Collections.Generic;
using UnityEngine;

public class InteractableTask : MonoBehaviour, IAgentTask
{
    [SerializeField] private InteractableObj interactWith;
    [SerializeField] private int maxWorkers = 1;
    [SerializeField] private float priorityOverTime = 50f;
    [SerializeField] private WorkerPriority minWorkerPriority;
    [SerializeField] private WorkerPriority maxWorkerPriority;

    private const float BasePriority = 1f;
    private const float WorkerSpeed = 5f;
    
    private HashSet<DuskwatchAgent> _currentWorkers = new HashSet<DuskwatchAgent>();
    private float _enableTime;

    private void OnEnable()
    {
        SceneReferences.Instance.WorkerTaskManager.RegisterTask(this);
        _enableTime = Time.time;
    }
    private void OnDisable()
    {
        SceneReferences.Instance.WorkerTaskManager.DeregisterTask(this);
    }

    public bool CanPerformTask(DuskwatchAgent agent)
    {
        return _currentWorkers.Count < maxWorkers;
    }

    public float GetPriority(DuskwatchAgent agent)
    {
        float time = Time.time - _enableTime;
        if (time < 0.5f) return BasePriority;
        float distance = Distance(agent.Position);
        if (interactWith.Interacting && interactWith.RemainingTime < distance/WorkerSpeed) return BasePriority;
        float timePriority = time * interactWith.RemainingTime * priorityOverTime;
        float workerPriority = WorkerPriority.GetWorkerPriority(minWorkerPriority, maxWorkerPriority, _currentWorkers.Count);
        return (workerPriority+timePriority)-distance;
    }

    public float Distance(Vector3 agentPosition)
    {
        return Vector3.Distance(interactWith.Position, agentPosition);
    }

    public void StartTask(DuskwatchAgent worker)
    {
        _currentWorkers.Add(worker);
        worker.QueueAction(new MoveAgentAction(worker, () => interactWith.GetClosestPosition(worker.Position)));
        worker.QueueAction(new InteractableAction(worker, interactWith));
        worker.QueueAction(new WaitAction(0.3f));
        worker.SetWeapon(WeaponType.Hammer);
    }

    public void StopTask(DuskwatchAgent worker)
    {
        _currentWorkers.Remove(worker);
    }
}
