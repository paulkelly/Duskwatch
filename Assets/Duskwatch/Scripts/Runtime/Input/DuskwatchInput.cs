using System;
using UnityEngine;

public static class DuskwatchInput
{
    private static bool _init;
    private static InputSystem_Actions _actions;
    
    public static InputSystem_Actions Actions
    {
        get
        {
            if (!_init)
            {
                _actions = new InputSystem_Actions();
                SetInputMode(InputMode.Default);
                _actions.Cursor.Enable();
                _init = true;
            }
            return _actions;
        }
    }

    private static ControllerType _controllerType;
    public delegate void ControllerTypeChanged(ControllerType type);
    public static event ControllerTypeChanged OnControllerTypeChanged;

    public static ControllerType ControllerType => _controllerType;
    public static void SetControllerType(ControllerType type)
    {
        if(_controllerType == type) return;
        
        _controllerType = type;
        OnControllerTypeChanged?.Invoke(_controllerType);
    }

    private static InputMode _inputMode;
    public static InputMode InputMode => _inputMode;
    public static void SetInputMode(InputMode inputMode)
    {
        _inputMode = inputMode;
        switch (_inputMode)
        {
            case InputMode.Default:
                _actions.Player.Enable();
                _actions.Build.Disable();
                break;
            case InputMode.Build:
                _actions.Player.Disable();
                _actions.Build.Enable();
                break;
            default:
                break;
        }
    }

    public static void SetPaused(bool paused)
    {
        
    }
}

public enum InputMode
{
    Default,
    Build
}

public enum ControllerType
{
    KeyboardAndMouse,
    Gamepad
}
