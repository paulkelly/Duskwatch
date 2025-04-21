using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "BuildingSettings", menuName = "Scriptable Objects/BuildingSettings")]
public class BuildingSettings : ScriptableObject
{
    public string name;
    public Sprite icon;
    [AssetsOnly] public GameObject prefab;
    public float constructionTime;
    public int housing;

    public List<ResourceRequirement> resourceRequirements;
    
    
    public bool HasResourceRequirements()
    {
        foreach (var resourceRequirement in resourceRequirements)
        {
            if (!resourceRequirement.IsRequirementMet())
            {
                return false;
            }
        }

        return true;
    }
}
