using System;
using Sirenix.OdinInspector;
using UnityEngine;

public class ReturnResourceComponent : MonoBehaviour, IBuildingActiveFunctions
{
    [SerializeField] private Collider _collider;
    [SerializeField] private bool _allowAny;
    [SerializeField, HideIf("_allowAny")] private ResourceDefinition _allowedResource;

    public Vector3 Position { get; private set; }
    public Collider Collider => _collider;
    public bool AllowAny => _allowAny;
    public ResourceDefinition AllowedResource => _allowedResource;

    private void OnEnable()
    {
        Position = transform.position;
    }
    
    public void OnBuildingActive()
    {
        SceneReferences.Instance.BuildingManager.RegisterResourceCollector(this);
    }

    public void OnBuildingInactive()
    {
        SceneReferences.Instance.BuildingManager.DeregisterResourceCollector(this);
    }
}
