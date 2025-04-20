using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterInputHandler : MonoBehaviour, InputSystem_Actions.IPlayerActions
{
    [SerializeField] private CharacterMovement _characterMovement;
    [SerializeField] private InteractableManager _interactManager;
    
    [SerializeField] private Animator _animator;
    private static readonly int AttackAnimationHash = Animator.StringToHash("Attack");
    
    private InputSystem_Actions _input;
    
    private void Start()
    {
        _input = DuskwatchInput.Actions;
        _input.Player.AddCallbacks(this);
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        _characterMovement.MoveInput = context.ReadValue<Vector2>();
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            _interactManager.Interact();
        }
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            _animator.SetTrigger(AttackAnimationHash);
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
