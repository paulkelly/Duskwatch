using System.Collections.Generic;
using DataBinding;
using UnityEngine;
using UnityEngine.Pool;

public class InteractableTagPanel : MonoBehaviour
{
    [SerializeField] private GameObject _statusTagPrefab;
    private ObjectPool<InteractableStatusTag> statusTagPool;

    private Dictionary<InteractableObj, InteractableStatusTag> _activeTags = new Dictionary<InteractableObj, InteractableStatusTag>();

    private void Start()
    {
        statusTagPool = new ObjectPool<InteractableStatusTag>(CreateStatusTag, GetStatusTag, ReleaseStatusTag, DestoryStatusTag);
    }

    public void DisplayTag(InteractableObj obj)
    {
        if(obj == null) return;
        if(_activeTags.ContainsKey(obj)) return;

        var tag = statusTagPool.Get();
        _activeTags.Add(obj, tag);
        tag.Display(obj);
    }
    
    public void HideTag(InteractableObj obj)
    {
        if(!_activeTags.ContainsKey(obj)) return;
        _activeTags[obj].Hide();
        _activeTags.Remove(obj);
    }

    public void ReturnTag(InteractableStatusTag tag)
    {
        statusTagPool.Release(tag);
    }
    

    // Object Pool
    private InteractableStatusTag CreateStatusTag()
    {
        var go = Instantiate(_statusTagPrefab, transform);
        return go.GetComponent<InteractableStatusTag>();
    }
    
    private void GetStatusTag(InteractableStatusTag obj)
    {
    }
    
    private void ReleaseStatusTag(InteractableStatusTag obj)
    {
        obj.SetInactive();
    }
    
    private void DestoryStatusTag(InteractableStatusTag obj)
    {
        Destroy(obj.gameObject);
    }
    
}
