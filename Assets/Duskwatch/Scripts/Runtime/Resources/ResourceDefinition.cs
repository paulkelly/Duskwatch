using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "ResourceDefinition", menuName = "Scriptable Objects/ResourceDefinition")]
public class ResourceDefinition : ScriptableObject
{
    public string displayName;
    [PreviewField] public Sprite icon;
    public bool nonDepleting;

    [Unity.Collections.ReadOnly, ScriptableObjectId, SerializeField] private string resourceId;

    public string Id => resourceId;

    [Header("Worker Harvesting")] 
    public BackpackType backpackType;
    public WeaponType weaponType;
    public int damagePerHit;
    public int collectedPerHit;
    public int maxHeld;
    public int maxWorkersPerResource;
    public WorkerPriority minWorkers;
    public WorkerPriority maxWorkers;
}
