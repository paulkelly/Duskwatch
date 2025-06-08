using UnityEngine;

public class ReturnResourceAction : IAgentAction
{
    private WorkerAgent _worker;

    public ReturnResourceAction(WorkerAgent worker)
    {
        _worker = worker;
    }

    public bool Complete => true;
    public void Start()
    {
        _worker.ReturnResource();
    }
}