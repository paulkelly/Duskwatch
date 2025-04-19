using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.Rendering;
using UnityEngine.Serialization;
using @ReadOnly = global::Unity.Collections.ReadOnlyAttribute;

public class GridManager : MonoBehaviour
{
    [SerializeField] private Material _gridMaterial;
    [SerializeField] private Texture2D gridTexture;
    
    private static readonly int Position = Shader.PropertyToID("_Position");
    
    private LocalKeyword _showGridKeyword;
    
    private NativeArray<bool> blockedCells;
    private NativeArray<bool> blockedCellsBuffer; // separate array for passing into the job
    private NativeArray<Color32> gridTextureData;
    
    private JobHandle _updateTextureJobHandle;
    private bool _updateTextureJobRunning;
    private bool _disposed;
    private bool _blockedCellsUpdated;

    private void Awake()
    {
        _showGridKeyword = new LocalKeyword(_gridMaterial.shader, "_SHOWGRID_ON");
        
        blockedCells = new NativeArray<bool>(GridCell.GridBounds * GridCell.GridBounds, Allocator.Persistent);
        blockedCellsBuffer = new NativeArray<bool>(GridCell.GridBounds * GridCell.GridBounds, Allocator.Persistent);
            
        gridTextureData = gridTexture.GetRawTextureData<Color32>();

        //InitialiseGridWithTerrain();
    }

    private void Update()
    {
        if (_blockedCellsUpdated) // just update texture
        {
            Profiler.BeginSample("Copy blocked cell buffer");
            blockedCells.CopyTo(blockedCellsBuffer);
            Profiler.EndSample();
            
            var updateTextureJob = new UpdateGridTextureJob()
            {
                blockedCells = blockedCellsBuffer,
                texture = gridTextureData
            };
            _updateTextureJobHandle = updateTextureJob.Schedule(GridCell.GridBounds*GridCell.GridBounds, 8);
            _updateTextureJobRunning = true;

            _blockedCellsUpdated = false;
        }
    }

    private void LateUpdate()
    {
        Vector3 mousePos = SceneReferences.Instance.cursorInputHandler.MousePosition;
        Vector2 position = new Vector2(mousePos.x, mousePos.z);
        _gridMaterial.SetVector(Position, position);

        CompleteJobs();
    }

    private void CompleteJobs()
    {
        if (!_updateTextureJobRunning) return;
            
        Profiler.BeginSample("Complete Job");
        if(_updateTextureJobRunning) _updateTextureJobHandle.Complete();
        _updateTextureJobRunning = false;
        Profiler.EndSample();
            
        gridTexture.Apply();
    }
    
    private void OnDestroy()
    {
        _disposed = true;
        
        if (!_updateTextureJobHandle.IsCompleted)
        {
            _updateTextureJobHandle.Complete();
        }

        blockedCells.Dispose();
        blockedCellsBuffer.Dispose();
        gridTextureData.Dispose();
    }
    
    
    private bool _showGrid;
    public bool ShowGrid
    {
        get => _showGrid;
        set
        {
            _showGrid = value;
            _gridMaterial.SetKeyword(_showGridKeyword, value);
        }
    }
    public void SetBoundsBlocked(Bounds bounds, bool blocked, bool walkable)
    {
        if (_disposed) return;
            
        GridCell min = GridCell.FromWorldPos(bounds.min);
        GridCell max = GridCell.FromWorldPos(bounds.max);

        for (int x = min.x; x < max.x; x++)
        {
            for (int y = min.y; y < max.y; y++)
            {
                blockedCells[y * GridCell.GridBounds + x] = blocked;
            }
        }
        
        _blockedCellsUpdated = true;

        if (walkable) return;
            
        //SetWalkable(min, max, !blocked);
    }
    
    public bool IsPositionValid(Bounds bounds)
    {
        GridCell min = GridCell.FromWorldPos(bounds.min);
        GridCell max = GridCell.FromWorldPos(bounds.max);

        for (int x = min.x; x < max.x; x++)
        {
            for (int y = min.y; y < max.y; y++)
            {
                if (blockedCells[y * GridCell.GridBounds + x]) return false;
            }
        }

        return true;
    }
}

#region Update Texture Job
[BurstCompile]
public struct UpdateGridTextureJob : IJobParallelFor
{
    [@ReadOnly] public NativeArray<bool> blockedCells;
    public NativeArray<Color32> texture;
    public void Execute(int index)
    {
        byte r = blockedCells[index] ? byte.MaxValue : byte.MinValue;
        byte g = 0;
        byte b = 0;
        byte a = 1;
        texture[index] = new Color32(r, g, b, a);
    }
}
#endregion
