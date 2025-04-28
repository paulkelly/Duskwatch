using System;
using System.Collections.Generic;
using UnityEngine;

public class HarvestTaskManager : MonoBehaviour
{
    private const float DistanceThreshold = 50f;
    private HashSet<HarvestResourceTask> _harvestTasks = new HashSet<HarvestResourceTask>();

    public void RegisterHarvestTask(HarvestResourceTask task)
    {
        _harvestTasks.Add(task);
        CheckResourceTask(task);
    }

    public void DeregisterHarvestTask(HarvestResourceTask task)
    {
        task.IsActive = false;
        _harvestTasks.Remove(task);
    }

    private void CheckResourceTask(HarvestResourceTask task)
    {
        task.IsActive = SceneReferences.Instance.BuildingManager.HasResourceReturnBuildingInRange(task.Position, task.Resource, DistanceThreshold);
    }
    

    private void OnEnable()
    {
        SceneReferences.Instance.BuildingManager.OnCollectionBuildingAdded += BuildingManagerOnOnCollectionBuildingAdded;
        SceneReferences.Instance.BuildingManager.OnCollectionBuildingRemoved += BuildingManagerOnOnCollectionBuildingRemoved;
    }

    private void OnDisable()
    {
        if(SceneReferences.Instance == null || SceneReferences.Instance.BuildingManager == null) return;
        
        SceneReferences.Instance.BuildingManager.OnCollectionBuildingAdded -= BuildingManagerOnOnCollectionBuildingAdded;
        SceneReferences.Instance.BuildingManager.OnCollectionBuildingRemoved -= BuildingManagerOnOnCollectionBuildingRemoved;
    }

    private void BuildingManagerOnOnCollectionBuildingAdded(ReturnResourceComponent collectionbuilding)
    {
        foreach (var task in _harvestTasks)
        {
            if(task.IsActive) continue;
            CheckResourceTask(task);
        }
    }
    private void BuildingManagerOnOnCollectionBuildingRemoved(ReturnResourceComponent collectionbuilding)
    {
        foreach (var task in _harvestTasks)
        {
            if(!task.IsActive) continue;
            CheckResourceTask(task);
        }
    }
    
}
