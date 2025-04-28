using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class StatusTagPool<T> : MonoBehaviour
{
    [SerializeField] private GameObject _statusTagPrefab;
    private ObjectPool<AbstractStatusTag> statusTagPool;

    private Dictionary<T, AbstractStatusTag> _activeTags = new Dictionary<T, AbstractStatusTag>();

    private void Awake()
    {
        statusTagPool = new ObjectPool<AbstractStatusTag>(CreateStatusTag, GetStatusTag, ReleaseStatusTag, DestoryStatusTag);
    }

    public void DisplayTag(T obj)
    {
        if(obj == null) return;
        if(_activeTags.ContainsKey(obj)) return;

        var tag = statusTagPool.Get();
        _activeTags.Add(obj, tag);
        tag.Bind(obj);
    }
    
    public void HideTag(T obj)
    {
        if(!_activeTags.ContainsKey(obj)) return;
        _activeTags[obj].Hide();
        _activeTags.Remove(obj);
    }

    public void ReturnTag(AbstractStatusTag tag)
    {
        statusTagPool.Release(tag);
    }
    

    // Object Pool
    private AbstractStatusTag CreateStatusTag()
    {
        var go = Instantiate(_statusTagPrefab, transform);
        return go.GetComponent<AbstractStatusTag>();
    }
    
    private void GetStatusTag(AbstractStatusTag obj)
    {
    }
    
    private void ReleaseStatusTag(AbstractStatusTag obj)
    {
        obj.SetInactive();
    }
    
    private void DestoryStatusTag(AbstractStatusTag obj)
    {
        Destroy(obj.gameObject);
    }
    
}