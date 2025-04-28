using System.Collections.Generic;
using UnityEngine;

public interface IAgentTask
{
    bool CanPerformTask(DuskwatchAgent agent);
    float GetPriority(DuskwatchAgent agent);
    float Distance(Vector3 agentPosition);
    void StartTask(DuskwatchAgent agent);
    void StopTask(DuskwatchAgent agent);
}
