using UnityEngine;

public static class DuskwatchInput
{
    private static bool _init;
    private static InputSystem_Actions _actions;
    
    public static InputSystem_Actions Actions
    {
        get
        {
            if (!_init) _actions = new InputSystem_Actions();
            return _actions;
        }
    }
}
