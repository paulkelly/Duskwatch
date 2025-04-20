using System;
using UnityEngine;

public class Building : MonoBehaviour
{
    [SerializeField] private BuildingSettings _settings;
    [SerializeField] private Collider _collider;

    public Collider Collider => _collider;
    
    private bool _validBuildingPosition;
    private bool _isPlaced;
    
    private IBuildingPlacementFunctions[] _buildingPlacementListeners;
    private IBuildingPlacementValidFunction[] _buildingPlacementValidListeners;
    
    protected void OnEnable()
    {
        _buildingPlacementListeners = GetComponents<IBuildingPlacementFunctions>();
        _buildingPlacementValidListeners = GetComponents<IBuildingPlacementValidFunction>();
    }

    public void BeginPlacement()
    {
        foreach (var module in _buildingPlacementListeners)
        {
            module.OnBeginPlacement();
        }

        _collider.isTrigger = true;
    }

    public void CompletePlacement()
    {
        _isPlaced = true;
        _collider.isTrigger = false;
        SceneReferences.Instance.GridManager.SetBoundsBlocked(_collider.bounds, true, false);
        
        foreach (var module in _buildingPlacementListeners)
        {
            module.OnFinishPlacement();
        }
        
    }
    
    public bool ValidBuildingPosition
    {
        get => _validBuildingPosition;
        set
        {
            _validBuildingPosition = value;

            foreach (var buildingModule in _buildingPlacementValidListeners)
            {
                buildingModule.UpdateBuildingPlacementValid(_validBuildingPosition);
            }
        }
    }
    
    public bool IsPlacementValid()
    {
        bool validPosition = SceneReferences.Instance.GridManager.IsPositionValid(Collider.bounds);
        if (!validPosition) return false;

        // for (int i = 0; i < data.costs.Length; i++)
        // {
        //     if (!SceneReferences.Instance.resourceManager.CanAffordCost(data.costs[i])) return false;
        // }
            
        // foreach (var module in _buildingPlacementValidListeners)
        // {
        //     if (!module.IsValid()) return false;
        // }

        return true;
    }
}
