using UnityEngine;

public class UIReferences : MonoBehaviour
{
    public static UIReferences Instance { get; private set; }

    public SelectionWheelPanel SelectionWheel;
    
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
