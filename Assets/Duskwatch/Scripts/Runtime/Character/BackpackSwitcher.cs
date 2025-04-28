using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class BackpackSwitcher : MonoBehaviour
{
    [SerializeField] private List<BackpackObj> availableBackpacks;

    private Dictionary<BackpackType, GameObject> _backpacks = new Dictionary<BackpackType, GameObject>();
    private BackpackType _active;

    public BackpackType ActiveBackpack => _active;

    private void Start()
    {
        foreach (var weaponTransform in availableBackpacks)
        {
            _backpacks.Add(weaponTransform.type, weaponTransform.gameObject);
            if (weaponTransform.gameObject.activeSelf) _active = weaponTransform.type;
        }
    }

    public void SetBackpack(BackpackType type)
    {
        if (type == BackpackType.None)
        {
            DisableAllBackpacks();
            return;
        }
        
        if(!_backpacks.ContainsKey(type)) return;
        DisableAllBackpacks();
        EnableBackpack(type);
    }
    
    public void EnableBackpack(BackpackType type)
    {
        if(!_backpacks.ContainsKey(type)) return;
        _backpacks[type].SetActive(true);
        _active = type;
    }
    
    public void DisableBackpack(BackpackType type)
    {
        if(!_backpacks.ContainsKey(type)) return;
        _backpacks[type].SetActive(true);
        if (_active == type) _active = BackpackType.None;
    }

    public void DisableAllBackpacks()
    {
        foreach (var backpackType in availableBackpacks)
        {
            backpackType.gameObject.SetActive(false);
        }

        _active = BackpackType.None;
    }
}

[Serializable]
public enum BackpackType
{
    None,
    Backpack,
    Wood,
    Quiver
}

[Serializable]
public struct BackpackObj
{
    public BackpackType type;
    public GameObject gameObject;
}