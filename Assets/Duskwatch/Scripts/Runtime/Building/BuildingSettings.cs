using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "BuildingSettings", menuName = "Scriptable Objects/BuildingSettings")]
public class BuildingSettings : ScriptableObject
{
    public string name;
    public Sprite icon;
    [AssetsOnly] public GameObject prefab;
    
    
    
    
    
}
