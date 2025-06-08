using System;
using UnityEngine;

public class ResourcePanel : MonoBehaviour
{
    [SerializeField] private GameObject _resourcePrefab;
    [SerializeField] private RectTransform _parent;
    public void Start()
    {
        foreach (var resource in SceneReferences.Instance.ResourceManager.GetAllResources())
        {
            if (resource.hidden) continue;
            var obj = Instantiate(_resourcePrefab, _parent);
            var display = obj.GetComponent<ResourceDisplay>();
            if (display == null)
            {
                Debug.LogError("Unable to load Resource UI");
                Destroy(obj);
                continue;
            }
            
            display.Bind(resource);
        }
    }
}
