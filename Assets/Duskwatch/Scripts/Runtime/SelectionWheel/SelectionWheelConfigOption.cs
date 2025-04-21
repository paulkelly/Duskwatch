using UnityEngine;

public abstract class SelectionWheelConfigOption : ScriptableObject
{
    public abstract string displayText { get; }
    public abstract Sprite icon { get; }
    public abstract bool OnSelect();
    public abstract ISelectionWheelRequirement[] GetRequirements { get; }
}
