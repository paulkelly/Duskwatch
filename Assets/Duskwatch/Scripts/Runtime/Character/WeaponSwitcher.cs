using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class WeaponSwitcher : MonoBehaviour
{
    [SerializeField] private List<WeaponObj> availableWeapons;

    private Dictionary<WeaponType, GameObject> _weapons = new Dictionary<WeaponType, GameObject>();
    private WeaponType _active;

    public WeaponType ActiveWeapon => _active;

    private void Start()
    {
        foreach (var weaponTransform in availableWeapons)
        {
            _weapons.Add(weaponTransform.type, weaponTransform.gameObject);
            if (weaponTransform.gameObject.activeSelf) _active = weaponTransform.type;
        }
    }

    public void SetWeapon(WeaponType type)
    {
        if (type == WeaponType.None)
        {
            DisableAllWeapons();
            return;
        }
        
        DisableAllWeapons();
        EnableWeapon(type);
    }
    
    public void EnableWeapon(WeaponType type)
    {
        if(!_weapons.ContainsKey(type)) return;
        _weapons[type].SetActive(true);
        _active = type;
    }
    
    public void DisableWeapon(WeaponType type)
    {
        if(!_weapons.ContainsKey(type)) return;
        _weapons[type].SetActive(true);
        if (_active == type) _active = WeaponType.None;
    }

    public void DisableAllWeapons()
    {
        foreach (var weaponTransform in availableWeapons)
        {
            weaponTransform.gameObject.SetActive(false);
        }
        _active = WeaponType.None;
    }
}

[Serializable]
public enum WeaponType
{
    None,
    Hammer,
    Axe,
    Bow,
    Pick
}

[Serializable]
public struct WeaponObj
{
    public WeaponType type;
    public GameObject gameObject;
}