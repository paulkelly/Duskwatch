using UnityEngine;

public class BuildingConstructionInteractable : InteractableObj
{
    [SerializeField] private Building _building;
    public override float InteractTime => _building.ConstructionTime;

    public override void OnProgressUpdated()
    {
        _building.SetConstructionProgress(Mathf.Clamp01(Progress / InteractTime));
    }
}
