using System;
using UnityEngine;

public class CharacterBuildController : MonoBehaviour
{
    [SerializeField] private CharacterMovement _movement;
    [SerializeField] private Animator _animator;
    [SerializeField] private WeaponSwitcher _weaponSwitcher;
    private static readonly int BuildingProperty = Animator.StringToHash("Building");

    private WeaponType _previousWeapon;

    private void OnEnable()
    {
        BuildingConstructionInteractable.OnBeginBuilding += StartBuilding;
        BuildingConstructionInteractable.OnStopBuilding += StopBuilding;
    }

    private void OnDisable()
    {
        BuildingConstructionInteractable.OnBeginBuilding -= StartBuilding;
        BuildingConstructionInteractable.OnStopBuilding -= StopBuilding;
    }

    public void StartBuilding(Building target)
    {
        _animator.SetBool(BuildingProperty, true);
        _previousWeapon = _weaponSwitcher.ActiveWeapon;
        _weaponSwitcher.SetWeapon(WeaponType.Hammer);
        _movement.LockPosition(target.Collider.ClosestPoint(transform.position), target.transform.position);
    }

    public void StopBuilding(Building target)
    {
        _animator.SetBool(BuildingProperty, false);
        _weaponSwitcher.SetWeapon(_previousWeapon);
        _movement.UnlockPosition();
    }
    
}
