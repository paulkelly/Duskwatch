using System;
using UnityEngine;
using UnityEngine.Serialization;

public class SceneReferences : MonoBehaviour
{
    public static SceneReferences Instance { get; private set; }

    public Transform RuntimeRoot;
    public GridManager GridManager;
    public BuildSystem BuildSystem;
    public BuildingManager BuildingManager;
    public ResourceManager ResourceManager;
    [FormerlySerializedAs("MouseInputHandler")] public CursorInputHandler cursorInputHandler;

    public WorkerTaskManager WorkerTaskManager;
    public HarvestTaskManager HarvestTaskManager;
    public WaveManager WaveManager;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            #if DEBUG
            Debug.LogError("Duplicate SceneReferences: DELETING");
            #endif
            return;
        }
        Instance = this;
    }
}
