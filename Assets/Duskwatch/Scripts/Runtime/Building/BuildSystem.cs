using System;
using DataBinding;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;

public class BuildSystem : MonoBehaviour, InputSystem_Actions.IBuildActions
{
    private const float ConfirmPlacementTime = 0.5f;
    
    [SerializeField] private Texture2D objectPlacementMap;
    
    private InputSystem_Actions _input;
    
    public bool InPlacementMode { get; private set; }
    private bool _hasTextureChanges;
    
    private GameObject _currentPlacement;

    private BuildingSettings _currentBuildingSettings;
    private Building _currentPlacementBuilding;
    private float _currentConfirmationTime;

    public ButtonConfirmationFeedback ConfirmPlacementFeedback { get; private set; }
    public Vector3 CurrentPlacementPosition { get; private set; }

    private void Awake()
    {
        ConfirmPlacementFeedback = new ButtonConfirmationFeedback();
    }

    private void Start()
    {
        _input = DuskwatchInput.Actions;
        _input.Build.AddCallbacks(this);

        ClearMap();
    }

    private void Update()
    {
        bool confirmButtonPressed = _input.Build.Build.IsPressed();

        if (!confirmButtonPressed)
        {
            UpdateCurrentBuildingPosition();
        }
        
        // ConfirmPlacementFeedback.IsPressed.SetValue(confirmButtonPressed);
        //
        // if (confirmButtonPressed && _currentPlacementBuilding.ValidBuildingPosition)
        // {
        //     _currentConfirmationTime += Time.deltaTime;
        //     if (_currentConfirmationTime > ConfirmPlacementTime)
        //     {
        //         PlaceBuilding();
        //     }
        // }
        // else
        // {
        //     _currentConfirmationTime = 0f;
        // }
        //
        // ConfirmPlacementFeedback.NormalConfirmationTime.SetValue(Mathf.Clamp01(_currentConfirmationTime / ConfirmPlacementTime));
    }

    public void BeginBuildingPlacement(BuildingSettings buildingSettings)
    {
        if (InPlacementMode) return;

        _currentBuildingSettings = buildingSettings;
        StartPlacement();
    }
    
    private void UpdateCurrentBuildingPosition()
    {
        if (!InPlacementMode) return;
        
        Vector3 mousePos = SceneReferences.Instance.cursorInputHandler.MousePosition;
        Bounds bounds = _currentPlacementBuilding.Collider.bounds;
        CurrentPlacementPosition = GridUtils.GetCenterPosition(mousePos, bounds);
        _currentPlacement.transform.position = CurrentPlacementPosition;
            
        Physics.SyncTransforms();
            
        //TODO: Validate Placement
        _currentPlacementBuilding.ValidBuildingPosition = _currentPlacementBuilding.IsPlacementValid();


        RemovePreviousTextureChanges();
        ApplyObjectTextureChanges();
    }

    private void PlaceBuilding()
    {
        if (!_currentPlacementBuilding.ValidBuildingPosition)
        {
            return;
        }

        _currentPlacementBuilding.CompletePlacement();
        
        _currentPlacementBuilding = null;
        _currentPlacement = null;
        
        StartPlacement();
    }

    private void CancelPlacement()
    {
        StopPlacement();

        _currentPlacementBuilding = null;
        Destroy(_currentPlacement);
    }

    private void StartPlacement()
    {
        if(_currentBuildingSettings == null) return;
        
        InPlacementMode = true;
        DuskwatchInput.SetInputMode(InputMode.Build);
        SceneReferences.Instance.GridManager.ShowGrid = true;
        
        _currentPlacement = Instantiate(_currentBuildingSettings.prefab);
        _currentPlacementBuilding = _currentPlacement.GetComponent<Building>();
        _currentPlacementBuilding.BeginPlacement();
        
        UpdateCurrentBuildingPosition();
    }

    private void StopPlacement()
    {
        InPlacementMode = false;
        DuskwatchInput.SetInputMode(InputMode.Default);
        SceneReferences.Instance.GridManager.ShowGrid = false;
    }
    
    
    private GridCell previousMin;
    private GridCell previousMax;
        
    private void ApplyObjectTextureChanges()
    {
        if (_hasTextureChanges)
        {
            SetTexture(previousMin, previousMax, Color.black);
        }

        var bounds = _currentPlacementBuilding.Collider.bounds;
        GridCell min = GridCell.FromWorldPos(bounds.min);
        GridCell max = GridCell.FromWorldPos(bounds.max);
        SetTexture(min, max, Color.red);
        previousMin = min;
        previousMax = max;
        objectPlacementMap.Apply();
        _hasTextureChanges = true;
    }

    public void RemovePreviousTextureChanges()
    {
        if (_hasTextureChanges)
        {
            SetTexture(previousMin, previousMax, Color.black);
            objectPlacementMap.Apply();
            _hasTextureChanges = false;
        }
    }

    private void SetTexture(GridCell min, GridCell max, Color color)
    {
        int width = max.x - min.x;
        int height = max.y - min.y;

        Color[] colors = new Color[width * height];

        if (color != Color.black)
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    colors[y * width + x] = color;
                }
            }
        }

        objectPlacementMap.SetPixels(min.x, min.y, width, height,colors);
    }
        
    [Button]
    private void ClearMap()
    {
        objectPlacementMap.SetPixels(new Color[objectPlacementMap.width*objectPlacementMap.height]);
        objectPlacementMap.Apply();
    }
    
    
    // Input
    public void OnBuild(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            PlaceBuilding();
        }
    }

    public void OnCancel(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            CancelPlacement();
        }
    }
}
