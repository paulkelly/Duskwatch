using System;
using UnityEngine;

public class HousingComponent : MonoBehaviour, IBuildingActiveFunctions
{
    [SerializeField] private Building _building;
    [SerializeField] private ResourceDefinition _housingResource;
    public void OnBuildingActive()
    {
        SceneReferences.Instance.ResourceManager.AddResource(_housingResource, _building.HousingProvided);
    }

    public void OnBuildingInactive()
    {
        SceneReferences.Instance.ResourceManager.RemoveResource(_housingResource, _building.HousingProvided);
    }

    protected void Reset()
    {
        _building = GetComponent<Building>();
    }
}
