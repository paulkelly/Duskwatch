using UnityEngine;

public class BuildingConstruction : MonoBehaviour, IBuildingPlacementFunctions
{
    [SerializeField] private BuildingConstructionInteractable _interactable;
    public void OnBeginPlacement()
    {
    }

    public void OnCancelPlacement()
    {
    }

    public void OnFinishPlacement()
    {
        _interactable.gameObject.SetActive(true);
    }

    public void ConstructionProgressUpdated(float progress)
    {
    }

    public void OnFinishConstruction()
    {
        _interactable.gameObject.SetActive(false);
    }
}
