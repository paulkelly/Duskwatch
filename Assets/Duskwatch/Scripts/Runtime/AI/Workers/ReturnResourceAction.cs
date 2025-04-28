using UnityEngine;

public class ReturnResourceAction : IAgentAction
{
    private DuskwatchAgent _worker;

    public ReturnResourceAction(DuskwatchAgent worker)
    {
        _worker = worker;
    }

    public bool Complete => true;
    public void Start()
    {
        _worker.ReturnResource();
    }
}