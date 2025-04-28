using System;
using Pathfinding.RVO;
using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterMovement : MonoBehaviour
{ 
    [SerializeField] private Animator _animator;
    [SerializeField] private CharacterController _characterController;
    [SerializeField] private RVOController _rvoController;

    private const float MovementSpeed = 6f;
    private static readonly int SpeedAnimationHash = Animator.StringToHash("Speed");
    
    private Camera _mainCamera;
    private Transform _mainCameraTransform;

    public Vector2 MoveInput { get; set; }
    
    private Vector3 _velocity;
    private Vector3 _accelerationVector;
    
    private Quaternion _rotation;
    private Quaternion _targetRotation;
    private Quaternion _rotationVelocityVector;

    private bool _lockPosition;
    private Vector3 _lockedPosition;
    private Vector3 _lockedRotationTarget;
    private const float LockPositionStopThreshold = 0.6f;
    private const float LockPositionMaxSpeedThreshold = 1.2f;

    public void LockPosition(Vector3 positionTarget, Vector3 rotationTarget)
    {
        _lockPosition = true;
        _lockedPosition = positionTarget;
        _lockedRotationTarget = rotationTarget;
    }

    public void UnlockPosition()
    {
        _lockPosition = false;
    }

    private void Start()
    {
        _mainCamera = Camera.main;
        _mainCameraTransform = _mainCamera.transform;
    }
    
    private void Update()
    {
        if (_lockPosition)
        {
            UpdateVelocityAndRotationToMoveLockedPosition();
        }
        else
        {
            UpdateVelocityAndRotationBasedOnInput();
        }

        _characterController.Move(_velocity * Time.deltaTime);
        transform.rotation = _rotation;

        float speed = Mathf.Clamp01(_velocity.magnitude / MovementSpeed);
        _animator.SetFloat(SpeedAnimationHash, speed);
        
        _rvoController.velocity = _velocity;
    }

    private void UpdateVelocityAndRotationToMoveLockedPosition()
    {
        Vector3 currentPosition = transform.position;
        Vector3 targetPosition = new Vector3(_lockedPosition.x, currentPosition.y, _lockedPosition.z);

        Vector3 motionVector = Vector3.zero;
        float distance = Vector3.Distance(currentPosition, targetPosition);
        if (distance > LockPositionStopThreshold)
        {
            Vector3 toTarget = targetPosition - currentPosition;
            float speedValue = Mathf.InverseLerp(LockPositionStopThreshold, LockPositionMaxSpeedThreshold, distance)*MovementSpeed;
            motionVector = toTarget.normalized*speedValue;
        }

        Vector3 rotationTargetDir = _lockedRotationTarget - currentPosition;
        if (rotationTargetDir.magnitude > 0.1f)
        {
            _targetRotation = Quaternion.LookRotation(_lockedRotationTarget - currentPosition);
        }

        _velocity = Vector3.SmoothDamp(_velocity, motionVector, ref _accelerationVector, 0.1f);
        _rotation = QuaternionUtil.SmoothDamp(_rotation, _targetRotation, ref _rotationVelocityVector, 0.1f);
    }

    private void UpdateVelocityAndRotationBasedOnInput()
    {
        float movementMag = MoveInput.magnitude;
        Vector3 motionVector = Vector3.zero;

        if (movementMag > 0.1f)
        {
            motionVector = GetWorldInput(MoveInput, movementMag) * MovementSpeed;
            _targetRotation = Quaternion.LookRotation(motionVector);
        }

        _velocity = Vector3.SmoothDamp(_velocity, motionVector, ref _accelerationVector, 0.1f);
        _rotation = QuaternionUtil.SmoothDamp(_rotation, _targetRotation, ref _rotationVelocityVector, 0.1f);
    }
    
    private Vector3 GetWorldInput(Vector2 input, float magnitude)
    {
        Vector3 worldVector = (new Vector3(input.x, 0, input.y));
        worldVector = Quaternion.LookRotation(_mainCameraTransform.forward) * worldVector;
        worldVector = new Vector3(worldVector.x, 0, worldVector.z);
        worldVector = Vector3.ProjectOnPlane(worldVector, Vector3.up);
        return worldVector.normalized * magnitude;
    }
}
