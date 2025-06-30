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

    // Biến lưu trữ giá trị Input
    private Vector2 currentMoveInput;
    public bool isSprinting = false;

    [SerializeField] private Transform cameraTransform;

    [SerializeField] private Character Character;
    [SerializeField] private float rotationSpeed = 10f;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();

        inputActions = new PlayerControls();
        moveAction = inputActions.Player.Move;
        SprintAction = inputActions.Player.Sprint;

        if (cameraTransform == null)
        {
            cameraTransform = Camera.main.transform;
        }
    }
    private void OnEnable()
    {
        moveAction.Enable();
        moveAction.performed += OnMovePerformed;
        moveAction.canceled += OnMoveCanceled;

        SprintAction.Enable();
        SprintAction.performed += OnSprintPerformed;
        SprintAction.canceled += OnSprintCanceled;
    }
    private void OnDisable()
    {
        moveAction.Disable();
        moveAction.performed -= OnMovePerformed;
        moveAction.canceled -= OnMoveCanceled;

        SprintAction.Disable();
        SprintAction.performed -= OnSprintPerformed;
        SprintAction.canceled -= OnSprintCanceled;
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
        characterController.Move(animator.deltaPosition);
    }
}
