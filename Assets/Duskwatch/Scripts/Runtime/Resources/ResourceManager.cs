using System;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    [SerializeField] private ResourceDefinitionCollection _allResources;

    private Dictionary<ResourceDefinition, Resource> _resources = new Dictionary<ResourceDefinition, Resource>();

    private void Awake()
    {
        foreach (var resourceDef in _allResources.resources)
        {
            _resources.Add(resourceDef, new Resource(resourceDef));
        }
    }

    public List<Resource> GetAllResources()
    {
        List<Resource> result = new List<Resource>();
        result.AddRange(_resources.Values);
        return result;
    }

    public int GetResourceAvailability(ResourceDefinition resourceDefinition)
    {
        if (_resources.TryGetValue(resourceDefinition, out var resource))
        {
            if (resource.nonDepleting)
            {
                return resource.amount - resource.inUse;
            }
            return resource.amount;
        }

        return 0;
    }
    
    public bool HasResources(ResourceDefinition resourceDefinition, int required)
    {
        return GetResourceAvailability(resourceDefinition) >= required;
    }
    
    public void AddResource(ResourceDefinition resourceDefinition, int amount)
    {
        if (_resources.TryGetValue(resourceDefinition, out var resource))
        {
            resource.amount.SetValue(resource.amount + amount);
        }
    }

    public void PayResourceCost(ResourceDefinition resourceDefinition, int amount)
    {
        if (resourceDefinition.nonDepleting)
        {
            ClaimResource(resourceDefinition, amount);
        }
        else
        {
            RemoveResource(resourceDefinition, amount);
        }
    }
    
    public void RemoveResource(ResourceDefinition resourceDefinition, int amount)
    {
        if (_resources.TryGetValue(resourceDefinition, out var resource))
        {
            if (resource.amount > amount)
            {
                resource.amount.SetValue(resource.amount - amount); 
            }
            else
            {
                resource.amount.SetValue(0);   
            }
        }
    }

    public void ClaimResource(ResourceDefinition resourceDefinition, int amount)
    {
        if (_resources.TryGetValue(resourceDefinition, out var resource))
        {
            if (!resource.nonDepleting) return;

            if (resource.inUse + amount > resource.amount)
            {
#if DEBUG
                Debug.LogError("Trying to claim more of a resource than is available");
#endif
            }
            resource.inUse.SetValue(resource.inUse + amount);
        }
    }

    public void FreeResource(ResourceDefinition resourceDefinition, int amount)
    {
        if (_resources.TryGetValue(resourceDefinition, out var resource))
        {
            if (!resource.nonDepleting) return;

            resource.inUse.SetValue(resource.inUse - amount);
        }
    }
}
