using System;
using UnityEngine;

public class TownHallComponent : MonoBehaviour
{
    private void Start()
    {
        SceneReferences.Instance.BuildingManager.SetTownHall(this);
    }
}
