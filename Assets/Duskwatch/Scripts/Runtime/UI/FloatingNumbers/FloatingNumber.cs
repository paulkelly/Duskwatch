using System.Collections.Generic;
using PrimeTween;
using TMPro;
using UnityEngine;

public class FloatingNumber : MonoBehaviour
{
    private const float HeightChange = 6f;
    private const float Duration = 2.5f;
    private const float FadeDelay = 1.5f;
    
    private const float PunchStrength = 0.5f;
    private const float PunchDelay = 0.3f;
    
    [SerializeField] private TMP_Text _text;
    [SerializeField] private RectTransform _rectTransform;
    [SerializeField] private CanvasGroup _alpha;
    
    public Component Owner { get; set; }
    public int Value { get; set; }

    private Vector3 _targetPosition;
    private RectTransform _canvasTransform;
    private Camera _mainCamera;

    private List<Tween> _activeTweens = new List<Tween>();
    private Sequence _fadeSequence;
    
    public void ShowFloatingText(RectTransform canvas, string text, Color color, Vector3 position)
    {
        _canvasTransform = canvas;
        
        _text.text = text;
        _text.color = color;
        _targetPosition = position;
        
        _mainCamera = Camera.main;
        _alpha.alpha = 1;

        foreach (var tween in _activeTweens)
        {
            tween.Stop();
        }
        _activeTweens.Clear();
        _fadeSequence.Stop();
        
        _activeTweens.Add(Tween.PunchScale(_rectTransform, Vector3.one * PunchStrength, PunchDelay));
        _activeTweens.Add(Tween.Custom(_targetPosition, _targetPosition + Vector3.up * HeightChange, Duration, OnValueChange, Ease.InSine));
        _fadeSequence = Tween.Delay(FadeDelay).Chain(Tween.Alpha(_alpha, 0, Duration - FadeDelay)).OnComplete(ReturnToPool);
    }

    private void OnValueChange(Vector3 targetPosition)
    {
        Vector2 screenPoint = _mainCamera.WorldToScreenPoint(targetPosition);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(_canvasTransform, screenPoint, null, out var pos);
        _rectTransform.anchoredPosition = pos;
    }

    private void ReturnToPool()
    {
        UIReferences.Instance.FloatingNumberPanel.ReturnToPool(this);
    }
}
