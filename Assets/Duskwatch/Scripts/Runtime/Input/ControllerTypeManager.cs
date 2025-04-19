using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class ControllerTypeManager : MonoBehaviour
{
    private InputSystem_Actions _input;
    
    private void Start()
    {
        _input = DuskwatchInput.Actions;
        InputSystem.onAnyButtonPress.Call(Action);
    }

    private void Update()
    {
        if(DuskwatchInput.ControllerType == ControllerType.Gamepad) return;
        
        if (_input.Cursor.Move.ReadValue<Vector2>().magnitude > 0.1f)
        {
            DuskwatchInput.SetControllerType(ControllerType.Gamepad);
        }
    }

    private void Action(InputControl obj)
    {
        if (obj.device == Keyboard.current || obj.device == Mouse.current)
        {
            DuskwatchInput.SetControllerType(ControllerType.KeyboardAndMouse);
        }
        else
        {
            DuskwatchInput.SetControllerType(ControllerType.Gamepad);
        }
    }
}
