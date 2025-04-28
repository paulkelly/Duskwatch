using System;
using PrimeTween;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
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
        if (mode == InputMode.SelectionWheel)
        {
            Tween.GlobalTimeScale(0.1f, 0.3f);
        }
        else
        {
            Tween.GlobalTimeScale(1f, 0.3f);
        }
    }
}
