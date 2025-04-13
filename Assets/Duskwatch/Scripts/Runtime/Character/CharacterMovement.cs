using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterMovement : MonoBehaviour, InputSystem_Actions.IPlayerActions
{ 
    [SerializeField] private Animator _animator;
    [SerializeField] private CharacterController _characterController;

    private const float MovementSpeed = 8f;
    private static readonly int SpeedAnimationHash = Animator.StringToHash("Speed");
    private static readonly int AttackAnimationHash = Animator.StringToHash("Attack");
    
    private InputSystem_Actions _input;
    private Camera _mainCamera;
    private Transform _mainCameraTransform;

    private Vector2 _moveInput;
    
    private Vector3 _velocity;
    private Vector3 _accelerationVector;
    
    private Quaternion _rotation;
    private Quaternion _targetRotation;
    private Quaternion _rotationVelocityVector;

    private void Start()
    {
        _input = DuskwatchInput.Actions;
        _input.Player.Enable();
        _input.Player.AddCallbacks(this);

        _mainCamera = Camera.main;
        _mainCameraTransform = _mainCamera.transform;
    }
    private void Update()
    {
        float movementMag = _moveInput.magnitude;
        Vector3 motionVector = Vector3.zero;

        if (movementMag > 0.1f)
        {
            motionVector = GetWorldInput(_moveInput, movementMag) * (MovementSpeed * Time.deltaTime);
            _targetRotation = Quaternion.LookRotation(motionVector);
        }
        
        _velocity = Vector3.SmoothDamp(_velocity, motionVector, ref _accelerationVector, 0.1f);
        _rotation = QuaternionUtil.SmoothDamp(_rotation, _targetRotation, ref _rotationVelocityVector, 0.1f);

        _characterController.Move(_velocity);
        transform.rotation = _rotation;
        
        _animator.SetFloat(SpeedAnimationHash, movementMag);
    }
    
    private Vector3 GetWorldInput(Vector2 input, float magnitude)
    {
        Vector3 worldVector = (new Vector3(input.x, 0, input.y));
        worldVector = Quaternion.LookRotation(_mainCameraTransform.forward) * worldVector;
        worldVector = new Vector3(worldVector.x, 0, worldVector.z);
        worldVector = Vector3.ProjectOnPlane(worldVector, Vector3.up);
        return worldVector.normalized * magnitude;
    }


    // Input
    public void OnMove(InputAction.CallbackContext context)
    {
        _moveInput = context.action.ReadValue<Vector2>();
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        if(context.action.WasPerformedThisFrame()) _animator.SetTrigger(AttackAnimationHash);
    }
}
