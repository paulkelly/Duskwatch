using System;
using UnityEngine;

public class BuildingHealth : MonoBehaviour, IBuildingPlacementFunctions
{
    [SerializeField] private Building _building;
    [SerializeField] private Health _health;

    private float constructionProgress;



    public void OnBeginPlacement()
    {
    }

    public void OnCancelPlacement()
    {
    }

    public void OnFinishPlacement()
    {
    }

    public void ConstructionProgressUpdated(float progress)
    {
        int healthToAdd = Mathf.CeilToInt((progress - constructionProgress)*_health.maxHealth);
        
        _health.Heal(healthToAdd);
        
        constructionProgress = progress;
    }

    public void OnFinishConstruction()
    {
    }

    private void Reset()
    {
        _building = GetComponent<Building>();
        _health = GetComponent<Health>();
    }
}
