using Sirenix.OdinInspector;
using UnityEngine;

public class ReturnResourceTask : MonoBehaviour, IAgentTask
{
    [SerializeField] private ReturnResourceComponent _returnResourceComponent;
    private Vector3 position;

    private void Start()
    {
        position = transform.position;
        SceneReferences.Instance.WorkerTaskManager.RegisterTask(this);
    }

    public bool CanPerformTask(DuskwatchAgent worker)
    {
        if (!worker.HasResource) return false;
        if(_returnResourceComponent.AllowAny) return true;
        return worker.HeldResource == _returnResourceComponent.AllowedResource;
    }
    public float GetPriority(DuskwatchAgent worker)
    {
        if (worker.HasResource)
        {
            if (!worker.HoldingMaxAmount)
            {
                return 100f - Distance(worker.Position);
            }
            return 10000f - Distance(worker.Position);
        }
        return -100f;
    }
    public float Distance(Vector3 agentPosition)
    {
        return Vector3.Distance(position, agentPosition);
    }

    public void StartTask(DuskwatchAgent agent)
    {
        SceneReferences.Instance.WorkerTaskManager.AddWorkerCollectingResource(agent.HeldResource, agent);
        agent.QueueAction(new MoveAgentAction(agent, () => _returnResourceComponent.Collider.ClosestPoint(agent.Position)));
        agent.QueueAction(new ReturnResourceAction(agent));
    }

    public void StopTask(DuskwatchAgent agent)
    {
        SceneReferences.Instance.WorkerTaskManager.RemoveWorkerCollectingResource(agent.HeldResource, agent);
    }
}
