using System.Collections.Generic;
using UnityEngine;

public class BuildingRenderer : MonoBehaviour, IBuildingPlacementFunctions, IBuildingPlacementValidFunction
{
    [SerializeField] private Renderer[] renderers;
    [SerializeField] private Material buildingPlacementMaterial;
    [SerializeField] private Material buildingConstructionMaterial;
        
    private static readonly int Valid = Shader.PropertyToID("_Valid");
    private static readonly int ConstructionProgress = Shader.PropertyToID("_Progress");
        
    private List<Material[]> _defaultMaterials;
        
    public void OnBeginPlacement()
    {
        _defaultMaterials = new List<Material[]>(renderers.Length);
        foreach (var renderer in renderers)
        {
            _defaultMaterials.Add(renderer.sharedMaterials);
                
            var swapMats = renderer.sharedMaterials;
            for(int i=0; i<swapMats.Length; i++)
            {
                swapMats[i] = buildingPlacementMaterial;
            }
            renderer.sharedMaterials = swapMats;
        }
    }

    public void OnCancelPlacement()
    {
    }

    public void OnFinishPlacement()
    {
        foreach (var renderer in renderers)
        {
            var swapMats = renderer.sharedMaterials;
            for(int i=0; i<swapMats.Length; i++)
            {
                swapMats[i] = buildingConstructionMaterial;
            }
            renderer.sharedMaterials = swapMats;
        }
    }

    public void ConstructionProgressUpdated(float progress)
    {
        foreach (var renderer in renderers)
        {
            foreach (var mat in renderer.materials)
            {
                mat.SetFloat(ConstructionProgress, progress);
            }
        }
    }

    public void OnFinishConstruction()
    {
        if (_defaultMaterials == null) return;
            
        for(int i=0; i<_defaultMaterials.Count; i++)
        {
            renderers[i].materials = _defaultMaterials[i];
        }
    }

    public bool IsValid()
    {
        return true;
    }

    public void UpdateBuildingPlacementValid(bool valid)
    {
        buildingPlacementMaterial.SetFloat(Valid, valid ? 1 : 0);
    }
}
