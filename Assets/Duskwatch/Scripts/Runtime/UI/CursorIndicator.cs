using System;
using PrimeTween;
using UnityEngine;
using UnityEngine.UI;

public class CursorIndicator : MonoBehaviour
{
    private const float SmoothTime = 0.03f;
    private const float HideTime = 0.2f;
    
    [SerializeField] private RectTransform _canvasTransform;
    [SerializeField] private RectTransform[] _indicators;
    [SerializeField] private CanvasGroup _canvasGroup;

    private bool _visible;
    
    private Vector2[] _positions;
    private Vector2[] _dampVel;

    private float _lastMoveTime;
    private Vector2 _lastScreenPosition;

    private void Start()
    {
        _positions = new Vector2[_indicators.Length];
        _dampVel = new Vector2[_indicators.Length];
    }

    private void Update()
    {
        bool visible = DuskwatchInput.ControllerType == ControllerType.Gamepad && DuskwatchInput.InputMode == InputMode.Build;
        Vector2 screenPos = SceneReferences.Instance.cursorInputHandler.MouseScreenPosition;
        
        if (visible)
        {
            if (Vector2.Distance(screenPos, _lastScreenPosition) > 0.1f)
            {
                _lastMoveTime = Time.time;
            }

            if (Time.time - _lastMoveTime > HideTime) visible = false;
            _lastScreenPosition = screenPos;
        }

        if (_visible != visible)
        {
            _visible = visible;

            if (_visible)
            {
                Tween.Alpha(_canvasGroup, 0, 1, 0.2f);
            }
            else
            {
                Tween.Alpha(_canvasGroup, 1, 0, 0.2f);
            }
        }
        
        if(!_visible) return;
        for (int i = 0; i < _positions.Length ; i++)
        {
            if (i == 0)
            {
                _positions[i] = screenPos;
            }
            else
            {
                _positions[i] = Vector2.SmoothDamp(_positions[i], screenPos, ref _dampVel[i], i*SmoothTime);
            }
            
            
            RectTransformUtility.ScreenPointToLocalPointInRectangle(_canvasTransform, _positions[i], null, out var pos);
            _indicators[i].anchoredPosition = pos;
        }
    }
}
