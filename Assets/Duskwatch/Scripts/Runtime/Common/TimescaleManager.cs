using System;
using PrimeTween;
using UnityEngine;

public class TimescaleManager : MonoBehaviour
{
    private Tween _currentTween;
    private void OnEnable()
    {
        DuskwatchInput.OnInputModeChanged += DuskwatchInputOnOnInputModeChanged;
        DuskwatchInputOnOnInputModeChanged(DuskwatchInput.InputMode);
    }

    private void OnDisable()
    {
        DuskwatchInput.OnInputModeChanged -= DuskwatchInputOnOnInputModeChanged;
    }

    private void DuskwatchInputOnOnInputModeChanged(InputMode mode)
    {
        _currentTween.Stop();
        
        if (mode is InputMode.SelectionWheel or InputMode.Build)
        {
            _currentTween = Tween.GlobalTimeScale(0.1f, 0.3f);
        }
        else
        {
            _currentTween = Tween.GlobalTimeScale(1f, 0.3f);
        }
    }
}
