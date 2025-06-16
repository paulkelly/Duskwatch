using System;
using UnityEngine;

[Serializable]
public class SetEmissiveOnDayNightChange : IDayNightChange
{
    [SerializeField] private Material _material;
    public void OnDay()
    {
        _material.DisableKeyword("_EMISSION");
    }

    public void OnNight()
    {
        _material.EnableKeyword("_EMISSION");
    }
}
