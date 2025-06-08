using System;
using System.Collections.Generic;
using UnityEngine;

public class WorkerAgent : DuskwatchAgent
{
    [SerializeField] private ResourceDefinition _housingResource;
    [SerializeField] private BackpackSwitcher _backpackSwitcher;
    [SerializeField] private WeaponSwitcher _weaponSwitcher;
    
    private static readonly int HarvestAnimationHash = Animator.StringToHash("Harvest");
    
    private HeldResource _heldResource;
    
    public bool HasResource => _heldResource.amount > 0;
    public bool HoldingMaxAmount => _heldResource.amount >= _heldResource.resource.maxHeld;
    public ResourceDefinition HeldResource => _heldResource.resource;

    public bool Harvesting
    {
        get => _animator.GetBool(HarvestAnimationHash);
        set => _animator.SetBool(HarvestAnimationHash, value);
    }

    public void HarvestResource(ResourceDefinition resourceDefinition)
    {
        if (_heldResource.resource != resourceDefinition)
        {
            _heldResource = new HeldResource()
            {
                resource = resourceDefinition,
                amount = resourceDefinition.collectedPerHit
            };
        }
        else
        {
            _heldResource.amount = Mathf.Clamp(_heldResource.amount + resourceDefinition.collectedPerHit, 0, resourceDefinition.maxHeld);
        }

        _backpackSwitcher.SetBackpack(resourceDefinition.backpackType);
    }
    
    public void ReturnResource()
    {
        if(_heldResource.amount == 0) return;
        
        SceneReferences.Instance.ResourceManager.AddResource(_heldResource.resource, _heldResource.amount);
        UIReferences.Instance.FloatingNumberPanel.DisplayResourceGain(this, _heldResource.resource, _heldResource.amount, transform.position);
        _heldResource.amount = 0;
        
        _backpackSwitcher.SetBackpack(BackpackType.None);
    }

    public void SetWeapon(WeaponType weaponType)
    {
        _weaponSwitcher.SetWeapon(weaponType);
    }

    private void Start()
    {
        SceneReferences.Instance.ResourceManager.ClaimResource(_housingResource, 1);
    }

    private void OnDestroy()
    {
        SceneReferences.Instance.ResourceManager.FreeResource(_housingResource, 1);
    }

    public override void FindNewTask()
    {
        SetTask(SceneReferences.Instance.WorkerTaskManager.GetTask(this));
    }
}

public struct HeldResource
{
    public ResourceDefinition resource;
    public int amount;
}