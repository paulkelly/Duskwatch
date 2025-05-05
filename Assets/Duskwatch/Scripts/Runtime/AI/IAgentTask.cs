using System;
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

[Serializable]
public struct WorkerPriority
{
    public int workerCount;
    public float priority;

    public static float GetWorkerPriority(WorkerPriority min, WorkerPriority max, int workerCount)
    {
        float workerPriority = 1;
        if (workerCount < min.workerCount)
        {
            workerPriority = min.priority;
        }
        else if (workerCount > max.workerCount)
        {
            workerPriority = max.priority;
        }
        else
        {
            workerPriority = Mathf.Lerp(min.priority, max.priority, Mathf.InverseLerp(min.workerCount, max.workerCount, workerCount));
        }

        return workerPriority;
    }
}