using System;
using PrimeTween;
using UnityEngine;

public class SetActiveDestroyReaction : AbstractDestroyReaction
{
    [SerializeField] private bool _setEnabled;

    private Transform _transform;
    private Vector3 _defaultScale;
    private Vector3 _punchStrength;
    private const float TweenDuration = 0.5f;

    private void OnEnable()
    {
        _transform = GetComponent<Transform>();
        _defaultScale = _transform.localScale;
        _punchStrength = Vector3.one * 10f;
    }

    public override void OnDestroyed()
    {
        gameObject.SetActive(_setEnabled);
    }

    public override void OnResurrect()
    {
        gameObject.SetActive(!_setEnabled);
        
        if(_setEnabled) return;
        
        Tween.Scale(_transform, Vector3.zero, _defaultScale, TweenDuration);
        Tween.PunchLocalRotation(_transform, _punchStrength, TweenDuration, 2f);
    }
}
