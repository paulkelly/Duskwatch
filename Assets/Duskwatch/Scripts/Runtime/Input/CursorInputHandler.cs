using System;
using UnityEngine;

public class CursorInputHandler : MonoBehaviour
{
    private const int MaxHits = 20;
    private const float MaxDistance = 500;
    private const float ControllerCursorSpeed = 360f;
    
    private const float ControllerCursorDampingDuration = 1f;
    
    public Vector3 MousePosition { get; private set; }
    public Vector2 MouseScreenPosition { get; private set; }
    
    [SerializeField] private Camera _mainCamera;
    [SerializeField] private LayerMask groundLayers;
    
    private InputSystem_Actions _input;
    
    private readonly RaycastHit[] _hits = new RaycastHit[MaxHits];
    private int _hitCount;

    private bool _dampingCursor;
    private float _dampTime;

    private void Awake()
    {
        if(_mainCamera == null) _mainCamera = Camera.main;
    }

    private void Start()
    {
        _input = DuskwatchInput.Actions;
    }

    public void BeginControllerCursorDamping()
    {
        if(DuskwatchInput.ControllerType != ControllerType.Gamepad) return;

        _dampingCursor = true;
        _dampTime = 0;
    }

    public void Update()
    {
        if (DuskwatchInput.ControllerType == ControllerType.KeyboardAndMouse)
        {
            MouseScreenPosition = _input.Cursor.MousePosition.ReadValue<Vector2>();
        }
        else
        {
            if (DuskwatchInput.InputMode != InputMode.Build)
            {
                MouseScreenPosition = new Vector2(Screen.width / 2f, Screen.height / 2f);
            }
            else
            {
                Vector2 controllerInput = _input.Cursor.Move.ReadValue<Vector2>();
                controllerInput.x *= 1920f/Screen.width;
                controllerInput.y *= 1080f/Screen.height;

                if (_dampingCursor)
                {
                    _dampTime += Time.deltaTime;
                    float dampValue = _dampTime / ControllerCursorDampingDuration;
                    if (dampValue >= 1) _dampingCursor = false;

                    controllerInput *= Mathf.Lerp(0.3f, 1f, Mathf.Clamp01(dampValue));
                }
                
                MouseScreenPosition += controllerInput * (ControllerCursorSpeed * Time.deltaTime);
                MouseScreenPosition = new Vector2(Mathf.Clamp(MouseScreenPosition.x, 0, Screen.width), Mathf.Clamp(MouseScreenPosition.y, 0, Screen.height));
            }
            
        }

        Ray ray = _mainCamera.ScreenPointToRay(MouseScreenPosition);
            
        _hitCount = Physics.RaycastNonAlloc(ray.origin, ray.direction, _hits, MaxDistance, groundLayers);
        if (_hitCount > 0)
        {
            MousePosition = _hits[0].point;
        }
    }
}
