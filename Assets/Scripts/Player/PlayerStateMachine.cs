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

    // === Animator Hashes ===
    private readonly int isGroundedHash = Animator.StringToHash("isGrounded"); // Thêm dòng này

    // ==Gravity===
    [Header("Gravity")]
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] public float jumpForce = 5f; // Lực nhảy
    public float playerVelocityY;

    // === Animator Hashes ===
    private readonly int horizontalHash = Animator.StringToHash("Horizontal");
    private readonly int verticalHash = Animator.StringToHash("Vertical");
    private readonly int isSprintingHash = Animator.StringToHash("isSprinting");
    private readonly int isEquippedHash = Animator.StringToHash("isEquipped");
    private readonly int attackHash = Animator.StringToHash("Attack");

    // === Getters & Setters cho các State ===
    public PlayerBaseState CurrentState { get => currentState; set => currentState = value; }
    public CharacterController CharacterController { get => characterController; }
    public Animator Animator { get => animator; }
    public Vector2 CurrentMoveInput { get => currentMoveInput; }
    public bool IsAttackPressed { get => isAttackPressed; set => isAttackPressed = value; }
    public int HorizontalHash { get => horizontalHash; }
    public int VerticalHash { get => verticalHash; }
    public int IsSprintingHash { get => isSprintingHash; }
    public int IsEquippedHash { get => isEquippedHash; }
    public int AttackHash { get => attackHash; }
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
            animator.SetBool(isEquippedHash, false);
        }
    }

    void OnEnable() { inputActions.Player.Enable(); }
    void OnDisable() { inputActions.Player.Disable(); }

    void Update()
    {
        HandleGravity();

        animator.SetBool(isGroundedHash, characterController.isGrounded);

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
        animator.SetFloat(horizontalHash, currentMoveInput.x);
        animator.SetFloat(verticalHash, currentMoveInput.y);
        animator.SetBool(isSprintingHash, isSprinting);
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