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

    public bool CanPerformTask(DuskwatchAgent agent)
    {
        WorkerAgent worker = agent as WorkerAgent;
        if(!worker) return false;
        if (!worker.HasResource) return false;
        if(_returnResourceComponent.AllowAny) return true;
        return worker.HeldResource == _returnResourceComponent.AllowedResource;
    }
    public float GetPriority(DuskwatchAgent agent)
    {
        WorkerAgent worker = agent as WorkerAgent;
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
        WorkerAgent worker = agent as WorkerAgent;
        SceneReferences.Instance.WorkerTaskManager.AddWorkerCollectingResource(worker.HeldResource, agent);
        agent.QueueAction(new MoveAgentAction(worker, () => _returnResourceComponent.Collider.ClosestPoint(worker.Position)));
        agent.QueueAction(new ReturnResourceAction(worker));
    }

    public void StopTask(DuskwatchAgent agent)
    {
        WorkerAgent worker = agent as WorkerAgent;
        SceneReferences.Instance.WorkerTaskManager.RemoveWorkerCollectingResource(worker.HeldResource, agent);
    }
}
