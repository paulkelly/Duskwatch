using System.Collections.Generic;
using Pathfinding;
using UnityEngine;

public class TestTask : IAgentTask
{
    private Transform _targetPosition;

    public TestTask(Transform targetPosition)
    {
        _targetPosition = targetPosition;
    }

    public bool CanPerformTask(DuskwatchAgent agent)
    {
        return true;
    }

    public float GetPriority(DuskwatchAgent agent)
    {
        return 1;
    }

    public float Distance(Vector3 agentPosition)
    {
        return Vector3.Distance(_targetPosition.position, agentPosition);
    }

    public void StartTask(DuskwatchAgent agent)
    {
        agent.QueueAction(new MoveAgentAction(agent, () => _targetPosition.position));
        agent.QueueAction(new TestAction());
        
    }

    public void StopTask(DuskwatchAgent agent)
    {
    }
}
