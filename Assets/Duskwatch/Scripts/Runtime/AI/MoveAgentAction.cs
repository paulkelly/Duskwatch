using System;
using Pathfinding;
using UnityEngine;

public class MoveAgentAction : IAgentAction
{
    private const float MinRecalculateTime = 0.5f;
    
    private DuskwatchAgent _agent;
    private float _targetDistance;
    private float _lastRecalculateTime;

    private Func<Vector3> getTarget;
    
    public MoveAgentAction(DuskwatchAgent agent, Func<Vector3> target, float targetDistance = 0f)
    {
        _agent = agent;
        getTarget = target;
        _targetDistance = targetDistance;
    }

    public bool Complete => _agent.HasReachedDestination(_targetDistance);

    public void Start() => UpdateDestination();

    public void Update(float deltaTime)
    {
        if (Time.time - _lastRecalculateTime < MinRecalculateTime) return;
        
        Vector3 targetPosition = getTarget();
        if (Vector3.Distance(_agent.Destination, targetPosition) > 1f)
        {
            UpdateDestination();
        }
    }
    public void Stop() => _agent.Halt();

    private void UpdateDestination()
    {
        _lastRecalculateTime = Time.time;
        _agent.SetDestination(getTarget());
    }
}
