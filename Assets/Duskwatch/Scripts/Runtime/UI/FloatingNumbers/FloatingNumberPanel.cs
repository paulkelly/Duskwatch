using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class FloatingNumberPanel : MonoBehaviour
{
    [SerializeField] private GameObject _floatNumberPrefab;
    [SerializeField] private RectTransform _canvasTransform;

    [SerializeField] private Color _resourceGainColour;
    
    private ObjectPool<FloatingNumber> floatingNumberPool;

    private Dictionary<Component, FloatingNumber> _activeFloatingNumbers = new Dictionary<Component, FloatingNumber>();

    private void Awake()
    {
        floatingNumberPool = new ObjectPool<FloatingNumber>(CreateFloatingNumber, GetFloatingNumber, ReleaseFloatingNumber, DestroyFloatingNumber);
    }

    public void DisplayResourceGain(Component owner, ResourceDefinition definition, int amount, Vector3 position)
    {
        position += Vector3.up;
        
        if (_activeFloatingNumbers.TryGetValue(owner, out var number))
        {
            number.Value += amount;
            number.ShowFloatingText(_canvasTransform,$"+ {number.Value} {definition.displayName}", _resourceGainColour, position);
            return;
        }
        
        var floatingNumber = floatingNumberPool.Get();
        floatingNumber.Value = amount;
        floatingNumber.ShowFloatingText(_canvasTransform,$"+ {floatingNumber.Value} {definition.displayName}", _resourceGainColour, position);
        _activeFloatingNumbers.Add(owner, floatingNumber);
        floatingNumber.Owner = owner;
    }

    public void DisplayDamageNumber(float number, Vector3 position)
    {
        
    }

    public void ReturnToPool(FloatingNumber floatingNumber)
    {
        if (_activeFloatingNumbers.ContainsKey(floatingNumber.Owner))
        {
            _activeFloatingNumbers.Remove(floatingNumber.Owner);
        }
        floatingNumberPool.Release(floatingNumber);
    }
    
    
    // Object Pool
    private FloatingNumber CreateFloatingNumber()
    {
        var go = Instantiate(_floatNumberPrefab, transform);
        return go.GetComponent<FloatingNumber>();
    }
    
    private void GetFloatingNumber(FloatingNumber obj)
    {
        obj.gameObject.SetActive(true);
    }
    
    private void ReleaseFloatingNumber(FloatingNumber obj)
    {
        obj.gameObject.SetActive(false);
    }
    
    private void DestroyFloatingNumber(FloatingNumber obj)
    {
        Destroy(obj.gameObject);
    }
}
