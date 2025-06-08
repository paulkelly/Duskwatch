using System.Collections.Generic;
using UnityEngine;

public class BuildingManager : MonoBehaviour
{
    public delegate void CollectionBuildingAdded(ReturnResourceComponent collectionBuilding);
    public delegate void CollectionBuildingRemoved(ReturnResourceComponent collectionBuilding);

    public event CollectionBuildingAdded OnCollectionBuildingAdded;
    public event CollectionBuildingRemoved OnCollectionBuildingRemoved;
    
    private HashSet<ReturnResourceComponent> _resourceCollectionBuildings = new HashSet<ReturnResourceComponent>();
    
    public TownHallComponent townHall { get; private set; }

    public ReturnResourceComponent GetClosestReturnBuilding(Vector3 position, ResourceDefinition resource)
    {
        ReturnResourceComponent result = null;
        float best = Mathf.Infinity;
        foreach (var collectionBuilding in _resourceCollectionBuildings)
        {
            if (!collectionBuilding.AllowAny)
            {
                if(collectionBuilding.AllowedResource != resource) continue;
            }
            float distance = Vector3.Distance(collectionBuilding.Position, position);
            if (distance >= best) continue; 
            
            result = collectionBuilding;
            best = distance;
        }

        return result;
    }

    public bool HasResourceReturnBuildingInRange(Vector3 position, ResourceDefinition resource, float range)
    {
        foreach (var collectionBuilding in _resourceCollectionBuildings)
        {
            if (!collectionBuilding.AllowAny)
            {
                if(collectionBuilding.AllowedResource != resource) continue;
            }
            float distance = Vector3.Distance(collectionBuilding.Position, position);
            if (distance <= range) return true;
        }

        return false;
    }

    public void SetTownHall(TownHallComponent townHall)
    {
        this.townHall = townHall;
    }
    

    public void RegisterResourceCollector(ReturnResourceComponent collectionBuilding)
    {
        _resourceCollectionBuildings.Add(collectionBuilding);
        OnCollectionBuildingAdded?.Invoke(collectionBuilding);
    }
    
    public void DeregisterResourceCollector(ReturnResourceComponent collectionBuilding)
    {
        _resourceCollectionBuildings.Remove(collectionBuilding);
        OnCollectionBuildingRemoved?.Invoke(collectionBuilding);
    }
}
