using UnityEngine;

[RequireComponent(typeof(Building))]
public class PrePlacedBuilding : MonoBehaviour
{
    private void Start()
    {
        GetComponent<Building>().CompletePlacement();
    }
}
