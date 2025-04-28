using System;
using System.Collections.Generic;
using PrimeTween;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class SelectionWheelPanel : MonoBehaviour, InputSystem_Actions.ISelectionWheelActions
{
    private const float TweenTime = 0.3f;
    private const float TweenTimeFast = 0.1f;
    private const float InputStopTime = 0.2f;
    
    private const float StartAngle = 120f;

    [SerializeField] private SelectionWheelColourConfig _colourConfig;
    
    [SerializeField] private RectTransform _rectTransform;
    [SerializeField] private CanvasGroup _canvasGroup;

    [SerializeField] private Image _centerImage;
    [SerializeField] private Image _centerImageFade;
    [SerializeField] private RectTransform _requirementsNotMet;
    [SerializeField] private SelectionWheelRequirementsPanel _requirementsPanel;
    [SerializeField] private List<SelectionWheelSegment> _segments;
    
    [SerializeField] private CanvasGroup _noInputDisplay;
    [SerializeField] private CanvasGroup _selectionDisplay;
    [SerializeField] private TMP_Text _selectionText;
    
    [SerializeField] private RectTransform _arrow;
    [SerializeField] private RectTransform _arrowScale;
    [SerializeField] private CanvasGroup _arrowAlpha;

    [SerializeField] private SelectionWheelConfig _buildingSelectionWheel;
    
    private InputSystem_Actions _input;

    private Vector2 _moveInput;
    private Vector2 _arrowVector;
    private bool _hasInput;
    private float _lastInputTime;
    private float _selectionAngle;
    private int _selection = -1;
    private bool _hasSelection;

    public void ShowBuildingSelectionWheel()
    {
        ShowSelectionWheel(_buildingSelectionWheel);
    }

    public void ShowSelectionWheel(SelectionWheelConfig config)
    {
        Show();

        for(int i=0; i<_segments.Count; i++)
        {
            if (i < config.options.Length)
            {
                _segments[i].SetOption(config.options[i]);
            }
            else
            {
                _segments[i].SetOption(null);
            }

            _segments[i].Selected = false;
        }
    }

    private void Show()
    {
        _noInputDisplay.alpha = 1;
        _selectionDisplay.alpha = 0;
        Tween.Scale(_rectTransform, 1, TweenTime, Ease.Default, 1, CycleMode.Incremental, 0f, 0f, true);
        Tween.Alpha(_canvasGroup, 1, TweenTime, Ease.Default, 1, CycleMode.Incremental, 0f, 0f, true);

        DuskwatchInput.SetInputMode(InputMode.SelectionWheel);
    }
    private void Hide()
    {
        Tween.Scale(_rectTransform, 0, TweenTime, Ease.Default, 1, CycleMode.Incremental, 0f, 0f, true);
        Tween.Alpha(_canvasGroup, 0, TweenTime, Ease.Default, 1, CycleMode.Incremental, 0f, 0f, true);
        
        if(DuskwatchInput.InputMode == InputMode.SelectionWheel) DuskwatchInput.SetInputMode(InputMode.Default);
    }
    
    // Unity Functions
    private void Start()
    {
        _input = DuskwatchInput.Actions;
        _input.SelectionWheel.AddCallbacks(this);

        _canvasGroup.alpha = 0;
        _rectTransform.localScale = Vector3.zero;
    }

    private void LateUpdate()
    {
        if (DuskwatchInput.ControllerType == ControllerType.KeyboardAndMouse)
        {
            Vector2 centerPos = new Vector2(Screen.width / 2f, Screen.height / 2f);
            Vector2 mousePos = SceneReferences.Instance.cursorInputHandler.MouseScreenPosition;
            Vector2 dir = mousePos - centerPos;
            float distance = Vector2.Distance(mousePos, centerPos);
            if (distance > Screen.height/10f)
            {
                _moveInput = dir.normalized;
            }
            else
            {
                _moveInput = Vector2.zero;
            }
        }
        float inputMag = _moveInput.magnitude;

        bool hasInput = inputMag > 0.3f;

        if (_hasInput != hasInput)
        {
            _hasInput = hasInput;
            Tween.Alpha(_arrowAlpha, _hasInput ? 1 : 0, 0.1f, Ease.Default, 1, CycleMode.Incremental, 0f, 0f, true);
        }

        if (_hasInput)
        {
            _lastInputTime = Time.time;
            _arrowVector = _moveInput;

            _selectionAngle = -Vector2.SignedAngle(_arrowVector, Vector2.right);
            _arrow.rotation = Quaternion.AngleAxis(_selectionAngle, Vector3.forward);

            float angleOffset = StartAngle - _selectionAngle;
            if (angleOffset < 0) angleOffset += 360;
            SetSelection(Mathf.Clamp(Mathf.FloorToInt(angleOffset / 30f), 0, 11));
        }
        else if((Time.time-_lastInputTime) > InputStopTime)
        {
            SetSelection(-1);
        }
    }

    private void SetSelection(int selection)
    {
        if(_selection == selection) return;

        _selection = selection;
        _requirementsPanel.ClearRequirements();

        for (int i = 0; i < _segments.Count; i++)
        {
            _segments[i].Selected = _selection == i;
        }

        _hasSelection = _selection >= 0 && _segments[selection].HasOption;

        if (_hasSelection)
        {
            _selectionText.text = _segments[selection].Option.displayText;
            foreach (var requirement in _segments[selection].Option.GetRequirements)
            {
                _requirementsPanel.AddRequirement(requirement);
            }
        }

        Tween.Alpha(_noInputDisplay,_selection >= 0 ? 0 : 1, TweenTimeFast, Ease.Default, 1, CycleMode.Incremental, 0f, 0f, true);
        Tween.Alpha(_selectionDisplay,_hasSelection ? 1 : 0, TweenTimeFast, Ease.Default, 1, CycleMode.Incremental, 0f, 0f, true);
    }

    // INPUT
    
    #region Input Handling
    public void OnMove(InputAction.CallbackContext context)
    {
        _moveInput = context.ReadValue<Vector2>();
    }

    public void OnSelect(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (_selection < 0)
            {
                Hide();
                return;
            }

            Tween.PunchScale(_arrowScale, Vector3.one * 0.5f, 0.3f, 10f, true, Ease.Default, 0f, 1, 0f, 0f, true);
            
            if (_segments[_selection].HasOption)
            {
                bool selected = _segments[_selection].OnSelect();
                if (selected)
                {
                    Hide();
                }
                else
                {
                    _centerImage.color = _colourConfig.errorColour;
                    Tween.StopAll(_centerImage);
                    Tween.Color(_centerImage, _colourConfig.baseColourFade, 0.3f, Ease.Default, 1, CycleMode.Incremental, 0f, 0f, true);
                    
                    _centerImageFade.color = _colourConfig.errorColour;
                    Tween.StopAll(_centerImageFade);
                    Tween.Color(_centerImageFade, _colourConfig.baseColour, 0.3f, Ease.Default, 1, CycleMode.Incremental, 0f, 0f, true);

                    Tween.PunchScale(_requirementsNotMet, Vector3.one * 0.5f, 0.3f, 10f, true, Ease.Default, 0f, 1, 0f, 0f, true);
                }
            }
        }
    }

    public void OnCancel(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Hide();
        }
    }
    #endregion
}
