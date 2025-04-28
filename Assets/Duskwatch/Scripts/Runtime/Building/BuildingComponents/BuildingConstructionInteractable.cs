using UnityEngine;

public class BuildingConstructionInteractable : InteractableObj
{
    [SerializeField] private Building _building;
    public override float InteractTime => _building.ConstructionTime;

    public delegate void BeginBuilding(Building target);
    public delegate void StopBuilding(Building target);

    public static event BeginBuilding OnBeginBuilding;
    public static event StopBuilding OnStopBuilding;

    public override void OnProgressUpdated()
    {
        _building.SetConstructionProgress(Mathf.Clamp01(Progress / InteractTime));
    }

    protected override void OnInteractStart()
    {
        OnBeginBuilding?.Invoke(_building);
    }
    
    protected override void OnInteractEnd()
    {
        OnStopBuilding?.Invoke(_building);
    }
}
