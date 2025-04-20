using System;
using DataBinding;
using PrimeTween;
using UnityEngine;

public class BuildingPlacementConfirmation : MonoBehaviour
{
    [SerializeField] private RectTransform _canvasTransform;
    [SerializeField] private AbstractBinder _binder;
    [SerializeField] private GameObject _obj;
    [SerializeField] private RectTransform _rectTransform;

    private Camera _mainCamera;
    private bool _buttonShowing = false;

    private void Start()
    {
        _mainCamera = Camera.main;
    }

    private void Update()
    {
        bool inPlacementMode = SceneReferences.Instance.BuildSystem.InPlacementMode;
        if (_buttonShowing != inPlacementMode)
        {
            _buttonShowing = inPlacementMode;
            
            if (_buttonShowing)
            {
                //_binder.Bind(SceneReferences.Instance.BuildSystem.ConfirmPlacementFeedback);
                _obj.SetActive(true);
                Tween.StopAll(_obj.transform);
                Tween.Scale(_obj.transform, 0, 1, 0.1f);
            }
            else
            {
                Tween.StopAll(_obj.transform);
                Tween.Scale(_obj.transform, 1, 0, 0.1f).OnComplete(() => { _obj.SetActive(false); });
            }
        }
        
        if(!_buttonShowing) return;

        Vector3 screenPos = _mainCamera.WorldToScreenPoint(SceneReferences.Instance.BuildSystem.CurrentPlacementPosition);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(_canvasTransform, screenPos, null, out var pos);
        _rectTransform.anchoredPosition = pos;
    }

}
