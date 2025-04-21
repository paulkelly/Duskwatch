using UnityEngine;

public interface IBuildingPlacementFunctions
{
    public void OnBeginPlacement();
    public void OnCancelPlacement();
    public void OnFinishPlacement();
    public void ConstructionProgressUpdated(float progress);
    public void OnFinishConstruction();
}

public interface IBuildingPlacementValidFunction
{
    public bool IsValid();
    public void UpdateBuildingPlacementValid(bool valid);
}