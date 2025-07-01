using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.TextCore.Text;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(Animator))]
public class PlayerControler : MonoBehaviour
{
    // === Tham chiếu đến các component ===
    private CharacterController characterController;
    private Animator animator;

    // === Thông số di chuyển ===
    [SerializeField] private PlayerControls inputActions;
    private InputAction moveAction;
    private InputAction SprintAction;
    private InputAction attackAction;

    // Biến lưu trữ giá trị Input
    private Vector2 currentMoveInput;
    public bool isSprinting = false;

    [SerializeField] private Transform cameraTransform;

    [SerializeField] private Character Character;
    [SerializeField] private float rotationSpeed = 10f;

    // == thêm hằng số ID cho Animator Parameter ==
    private readonly int horizontalHash = Animator.StringToHash("Horizontal"); // Tham chiếu đến Animator Parameter "Horizontal"
    private readonly int verticalHash = Animator.StringToHash("Vertical"); // Tham chiếu đến Animator Parameter "Vertical"
    private readonly int isSprintingHash = Animator.StringToHash("isSprinting"); // Tham chiếu đến Animator Parameter "isSprinting"
    private readonly int isEquippeadHash = Animator.StringToHash("isEquipped"); // Tham chiếu đến Animator Parameter "isEquipped"
    private readonly int attackHash = Animator.StringToHash("Attack"); // Tham chiếu đến Animator Parameter "Attack"

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();

        inputActions = new PlayerControls();
        moveAction = inputActions.Player.Move;
        SprintAction = inputActions.Player.Sprint;
        attackAction = inputActions.Player.Attack;

        if (cameraTransform == null)
        {
            cameraTransform = Camera.main.transform;
        }
        animator.SetBool(isEquippeadHash, true);
    }
    private void OnEnable()
    {
        moveAction.Enable();
        moveAction.performed += OnMovePerformed;
        moveAction.canceled += OnMoveCanceled;

        SprintAction.Enable();
        SprintAction.performed += OnSprintPerformed;
        SprintAction.canceled += OnSprintCanceled;

        // kích hoạt Attack action 
        attackAction.Enable();
        attackAction.performed += OnAttackPerformed;
    }
    private void OnDisable()
    {
        moveAction.Disable();
        moveAction.performed -= OnMovePerformed;
        moveAction.canceled -= OnMoveCanceled;

        SprintAction.Disable();
        SprintAction.performed -= OnSprintPerformed;
        SprintAction.canceled -= OnSprintCanceled;

        attackAction.Disable();
        attackAction.performed -= OnAttackPerformed;
    }
    private void OnMovePerformed(InputAction.CallbackContext context)
    {
        currentMoveInput = context.ReadValue<Vector2>();        
    }
    private void OnMoveCanceled(InputAction.CallbackContext context)
    {
        currentMoveInput = Vector2.zero;
    }
    private void OnSprintPerformed(InputAction.CallbackContext context)
    {
        isSprinting = true;
    }
    private void OnSprintCanceled(InputAction.CallbackContext context)
    {
        isSprinting = false;
    }
    private void OnAttackPerformed(InputAction.CallbackContext context)
    {
        if(animator.GetBool(isEquippeadHash))
        {
            animator.SetTrigger(attackHash);
        }
    }
    private void Update()
    {
        HandleRotation();
        HandleMovement();     
        HandleAnimation();
    }
    private void HandleMovement()
    {
        Vector3 moveDirection = (transform.forward * currentMoveInput.y + transform.right * currentMoveInput.x).normalized;
        float currentSpeed = isSprinting ? Character.RunSpeed : Character.speed;
        characterController.Move(moveDirection * currentSpeed * Time.deltaTime);
    }
    private void HandleRotation()
    {
        Vector3 cameraForward = cameraTransform.forward;
        cameraForward.y = 0;
        cameraForward.Normalize();

        if(cameraForward != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(cameraForward);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }
    private void HandleAnimation()
    {
        animator.SetFloat("Horizontal", currentMoveInput.x);
        animator.SetFloat("Vertical", currentMoveInput.y);
        animator.SetBool("isSprinting", isSprinting);
    }
    private void OnAnimatorMove()
    {
        if (characterController != null)
        {
            characterController.Move(animator.deltaPosition);
        }
    }
}
