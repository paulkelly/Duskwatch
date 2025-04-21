using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterInputHandler : MonoBehaviour, InputSystem_Actions.IPlayerActions
{
    [SerializeField] private CharacterMovement _characterMovement;
    [SerializeField] private InteractableManager _interactManager;
    [SerializeField] private CharacterAttack _firstAttack;
    
    [SerializeField] private Animator _animator;
    private static readonly int AttackAnimationHash = Animator.StringToHash("Attack");
    
    private InputSystem_Actions _input;
    
    private void Start()
    {
        _input = DuskwatchInput.Actions;
        _input.Player.AddCallbacks(this);
    }

    private void Update()
    {
        _interactManager.InteractHeld = _input.Player.Interact.IsPressed();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        _characterMovement.MoveInput = context.ReadValue<Vector2>();
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            //_animator.SetTrigger(AttackAnimationHash);
            _firstAttack.Attack();
        }
    }

    public void OnBuild(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            UIReferences.Instance.SelectionWheel.ShowBuildingSelectionWheel();
        }
    }
}
