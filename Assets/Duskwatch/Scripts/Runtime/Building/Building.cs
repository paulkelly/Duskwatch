using System;
using UnityEngine;

public class Building : AbstractDestroyReaction
{
    [SerializeField] private BuildingSettings _settings;
    [SerializeField] private Collider _collider;

    public Collider Collider => _collider;
    public float ConstructionTime => _settings.constructionTime;
    public int HousingProvided => _settings.housing;
    
    private bool _validBuildingPosition;
    private bool _isPlaced;
    private bool _isConstructed;
    
    public float ConstructionProgress { get; private set; }

    private IBuildingActiveFunctions[] _buildingActiveListeners;
    private IBuildingPlacementFunctions[] _buildingPlacementListeners;
    private IBuildingPlacementValidFunction[] _buildingPlacementValidListeners;

    public bool BuildingActive { get; private set; }
    public void SetBuildingActive(bool active)
    {
        if(BuildingActive == active) return;
        BuildingActive = active;

        if (BuildingActive)
        {
            foreach (var module in _buildingActiveListeners)
            {
                module.OnBuildingActive();
            }
        }
        else
        {
            foreach (var module in _buildingActiveListeners)
            {
                module.OnBuildingInactive();
            }
        }
    }

    protected void OnEnable()
    {
        _buildingActiveListeners = GetComponents<IBuildingActiveFunctions>();
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

    public void CompleteConstruction()
    {
        if(!_isPlaced) CompletePlacement();

        SetBuildingActive(true);
        
        foreach (var module in _buildingPlacementListeners)
        {
            module.OnFinishConstruction();
        }
    }

    public void SetConstructionProgress(float progress)
    {
        ConstructionProgress = Mathf.Clamp01(progress);
        foreach (var module in _buildingPlacementListeners)
        {
            module.ConstructionProgressUpdated(ConstructionProgress);
        }
        if(progress >= 1) CompleteConstruction();
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

    public override void OnDestroyed()
    {
        SetBuildingActive(false);
    }

    public override void OnResurrect()
    {
        SetBuildingActive(true);
    }
}
