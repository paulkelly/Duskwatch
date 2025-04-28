using System;
using System.Collections.Generic;
using UnityEngine;

public class WorkerAgent : DuskwatchAgent
{
    [SerializeField] private ResourceDefinition _housingResource;

    private void Start()
    {
        SceneReferences.Instance.ResourceManager.ClaimResource(_housingResource, 1);
    }

    private void OnDestroy()
    {
        SceneReferences.Instance.ResourceManager.FreeResource(_housingResource, 1);
    }

    public override void FindNewTask()
    {
        SetTask(SceneReferences.Instance.WorkerTaskManager.GetTask(this));
    }
}

public struct HeldResource
{
    public ResourceDefinition resource;
    public int amount;
}