using UnityEngine;

public class MoveToTownCenterTask : IAgentTask
{
    public bool CanPerformTask(DuskwatchAgent agent)
    {
        return true;
    }

    public float GetPriority(DuskwatchAgent agent)
    {
        return 0;
    }

    public float Distance(Vector3 agentPosition)
    {
        return 0;
    }

    public void StartTask(DuskwatchAgent agent)
    {
        agent.QueueAction(new MoveAgentAction(agent, () => SceneReferences.Instance.BuildingManager.townHall.transform.position));
    }

    public void StopTask(DuskwatchAgent agent)
    {
    }
}
