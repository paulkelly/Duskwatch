using System;
using UnityEngine;

[Serializable]
public class ResourceRequirement : ISelectionWheelRequirement
{
    public ResourceDefinition resourceDefinition;
    public int required;

    public Sprite icon => resourceDefinition.icon;
    public int amount => required;
    public bool IsRequirementMet()
    {
        return SceneReferences.Instance.ResourceManager.HasResources(resourceDefinition, required);
    }
}
