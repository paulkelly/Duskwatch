using UnityEngine;

public interface ISelectionWheelRequirement
{
    public Sprite icon { get; }
    public int amount { get; }
    public bool IsRequirementMet();
}
