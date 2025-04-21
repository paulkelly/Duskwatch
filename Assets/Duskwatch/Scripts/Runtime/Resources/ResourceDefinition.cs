using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "ResourceDefinition", menuName = "Scriptable Objects/ResourceDefinition")]
public class ResourceDefinition : ScriptableObject
{
    public string displayName;
    [PreviewField] public Sprite icon;
    public bool nonDepleting;

    [Unity.Collections.ReadOnly, ScriptableObjectId, SerializeField] private string resourceId;

    public string Id => resourceId;
}
