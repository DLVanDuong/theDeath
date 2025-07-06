using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(EquipmentManager))]
public class PlayerStateMachine : MonoBehaviour
{
    // === State Machine ===
    private PlayerBaseState currentState;
    private PlayerStateFactory states;

    // === Tham chiếu ===
    public CharacterController characterController;
    private Animator animator;
    private EquipmentManager equipmentManager;

    // === Input ===
    private PlayerControls inputActions;
    private Vector2 currentMoveInput;
    public bool isSprinting = false;
    public bool isAttackPressed = false;
    public bool isJumpPressed = false;
    public bool isDodgePressed = false;

    // === Camera & Rotation ===
    [Header("Camera & Rotation")]
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private float rotationSpeed = 10f;

    // ==Gravity===
    [Header("Gravity")]
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] public float jumpForce = 5f; // Lực nhảy
    public float playerVelocityY;

    // === Animator Hashes ===
    private readonly int _horizontalHash = Animator.StringToHash("Horizontal");
    private readonly int _verticalHash = Animator.StringToHash("Vertical");
    private readonly int _isSprintingHash = Animator.StringToHash("isSprinting");
    private readonly int _isEquippedHash = Animator.StringToHash("isEquipped");
    private readonly int _attackHash = Animator.StringToHash("Attack");

    // === Getters & Setters cho các State ===
    public PlayerBaseState CurrentState { get => currentState; set => currentState = value; }
    public CharacterController CharacterController { get => characterController; }
    public Animator Animator { get => animator; }
    public Vector2 CurrentMoveInput { get => currentMoveInput; }
    public bool IsAttackPressed { get => isAttackPressed; set => isAttackPressed = value; }
    public int HorizontalHash { get => _horizontalHash; }
    public int VerticalHash { get => _verticalHash; }
    public int IsSprintingHash { get => _isSprintingHash; }
    public int IsEquippedHash { get => _isEquippedHash; }
    public int AttackHash { get => _attackHash; }
    public bool IsJumpPressed { get => isJumpPressed; set => isJumpPressed = value; }
    public bool IsDodgePressed { get => isDodgePressed; set => isDodgePressed = value; }
    public float PlayerVelocityY { get => playerVelocityY; set => playerVelocityY = value; }


    // === Testing ===
    [Header("Testing")]
    [Tooltip("Kéo các trang bị bạn muốn nhân vật tự mặc khi bắt đầu game vào đây.")]
    [SerializeField] private EquipmentData[] testWeapon;


    void Awake()
    {
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        equipmentManager = GetComponent<EquipmentManager>();
        states = new PlayerStateFactory(this);

        currentState = states.Grounded();
        currentState.EnterState();

        inputActions = new PlayerControls();
        inputActions.Player.Move.performed += ctx => currentMoveInput = ctx.ReadValue<Vector2>();
        inputActions.Player.Move.canceled += ctx => currentMoveInput = Vector2.zero;
        inputActions.Player.Sprint.performed += ctx => isSprinting = true;
        inputActions.Player.Sprint.canceled += ctx => isSprinting = false;
        inputActions.Player.Attack.performed += ctx => isAttackPressed = true;
        inputActions.Player.Jump.performed += ctx => isJumpPressed = true;
        inputActions.Player.Dodge.performed += ctx => isDodgePressed = true;

        if (cameraTransform == null) { cameraTransform = Camera.main.transform; }
     
        // --- Thêm logic để tự động trang bị vũ khí test ---
        if (testWeapon != null && testWeapon.Length > 0)
        {
            foreach (EquipmentData item in testWeapon)
            {
                if (item != null)
                {
                    equipmentManager.Equip(item);
                }
            }
        }
        else
        {
            animator.SetBool(_isEquippedHash, false);
        }
    }

    void OnEnable() { inputActions.Player.Enable(); }
    void OnDisable() { inputActions.Player.Disable(); }

    void Update()
    {
        HandleGravity();
        currentState.UpdateState();
    }
    // Hàm này áp dụng lực lên nhân vật
    void HandleGravity()
    {
        if (characterController.isGrounded && playerVelocityY < 0.0f)
        {
            playerVelocityY = -2.0f;
        }
        else
        {
            playerVelocityY += gravity * Time.deltaTime;
        }
        Vector3 verticalMovement = new Vector3(0, playerVelocityY, 0);
        characterController.Move(verticalMovement * Time.deltaTime);
    }
    public void HandleRotation()
    {
        Vector3 cameraForward = cameraTransform.forward;
        cameraForward.y = 0;
        cameraForward.Normalize();
        if (cameraForward != Vector3.zero)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(cameraForward), rotationSpeed * Time.deltaTime);
        }
    }

    public void HandleAnimation()
    {
        animator.SetFloat(_horizontalHash, currentMoveInput.x);
        animator.SetFloat(_verticalHash, currentMoveInput.y);
        animator.SetBool(_isSprintingHash, isSprinting);
    }

    private void OnAnimatorMove()
    {
        if (characterController != null)
        {
            characterController.Move(animator.deltaPosition);
        }
    }
    public void OnAttackAnimationEnd()
    {
        currentState.CheckSwitchStates();
    }
    public void OnDodgeAnimationEnd()
    {
        currentState.CheckSwitchStates();
    }
    public void EnableHitbox()
    {
        Debug.Log("Enabling hitbox for current weapon.");
        equipmentManager.EnableCurrentWeaponHitbox();
    }

    // Hàm này sẽ được gọi bởi Animation Event để TẮT hitbox
    public void DisableHitbox()
    {
        Debug.Log("Disabling hitbox for current weapon.");
        equipmentManager.DisableCurrentWeaponHitbox();
    }
}