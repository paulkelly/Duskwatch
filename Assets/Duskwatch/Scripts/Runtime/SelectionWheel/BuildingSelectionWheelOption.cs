using UnityEngine;

[CreateAssetMenu(fileName = "BuildingSelectionWheelOption", menuName = "Scriptable Objects/BuildingSelectionWheelOption")]
public class BuildingSelectionWheelOption : SelectionWheelConfigOption
{
    [SerializeField] private BuildingSettings _building;

    public override string displayText => _building.name;
    public override Sprite icon => _building.icon;
    public override bool OnSelect()
    {
        if (!_building.HasResourceRequirements())
        {
            return false;
        }
        SceneReferences.Instance.BuildSystem.BeginBuildingPlacement(_building);
        return true;
    }

    public override ISelectionWheelRequirement[] GetRequirements => _building.resourceRequirements.ToArray();
}
