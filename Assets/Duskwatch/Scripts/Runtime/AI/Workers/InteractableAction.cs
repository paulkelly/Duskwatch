using UnityEngine;

public class InteractableAction : IAgentAction
{
    private DuskwatchAgent _worker;
    private InteractableObj _interactWith;
    
    public InteractableAction(DuskwatchAgent worker, InteractableObj interactWith)
    {
        _worker = worker;
        _interactWith = interactWith;
    }

    public bool Complete => _interactWith.IsComplete;

    public void Start()
    {
        _worker.Harvesting = true;
        _worker.RotationTarget = _interactWith.transform;
        _interactWith.AddWorker(_worker);
    }

    public void Stop()
    {
        _worker.Harvesting = false;
        _worker.RotationTarget = null;
        _interactWith.RemoveWorker(_worker);
    }

    public void Update(float deltaTime)
    {
        _interactWith.UpdateProgress(deltaTime * 0.5f);
    }
}
