// PlayerStateMachine.cs
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
    private CharacterController characterController;
    private Animator animator;
    private EquipmentManager equipmentManager;

    // === Input ===
    private PlayerControls inputActions;
    private Vector2 currentMoveInput;
    public bool isSprinting = false;
    public bool isAttackPressed = false;

    // === Camera & Rotation ===
    [Header("Camera & Rotation")]
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private float rotationSpeed = 10f;

    // ==Gravity===
    [Header("Gravity")]
    [SerializeField] private float gravity = -9.81f;
    private float playerlVelocityY;

    // === Animator Hashes ===
    private readonly int _horizontalHash = Animator.StringToHash("Horizontal");
    private readonly int _verticalHash = Animator.StringToHash("Vertical");
    private readonly int _isSprintingHash = Animator.StringToHash("isSprinting");
    private readonly int _isEquippedHash = Animator.StringToHash("isEquipped");
    private readonly int _attackHash = Animator.StringToHash("Attack");

    // === Getters & Setters cho các State ===
    public PlayerBaseState CurrentState { get => currentState; set => currentState = value; }
    public Animator Animator { get => animator; }
    public Vector2 CurrentMoveInput { get => currentMoveInput; }
    public bool IsAttackPressed { get => isAttackPressed; set => isAttackPressed = value; }
    public int HorizontalHash { get => _horizontalHash; }
    public int VerticalHash { get => _verticalHash; }
    public int IsSprintingHash { get => _isSprintingHash; }
    public int IsEquippedHash { get => _isEquippedHash; }
    public int AttackHash { get => _attackHash; }

    // === Testing ===
    [Header("Testing")]
    [Tooltip("Kéo các trang bị bạn muốn nhân vật tự mặc khi bắt đầu game vào đây.")]
    [SerializeField] private EquipmentData[] testWeapon;


    void Awake()
    {
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        equipmentManager = GetComponent<EquipmentManager>();

        inputActions = new PlayerControls();
        inputActions.Player.Move.performed += ctx => currentMoveInput = ctx.ReadValue<Vector2>();
        inputActions.Player.Move.canceled += ctx => currentMoveInput = Vector2.zero;
        inputActions.Player.Sprint.performed += ctx => isSprinting = true;
        inputActions.Player.Sprint.canceled += ctx => isSprinting = false;
        inputActions.Player.Attack.performed += ctx => isAttackPressed = true;

        if (cameraTransform == null) { cameraTransform = Camera.main.transform; }

        states = new PlayerStateFactory(this);
        currentState = states.Idle();
        currentState.EnterState();

        // SỬA LỖI: Thay đổi logic xử lý mảng testWeapon
        // 1. Kiểm tra xem mảng có tồn tại và có phần tử nào không
        if (testWeapon != null && testWeapon.Length > 0)
        {
            // 2. Dùng vòng lặp để trang bị TỪNG vật phẩm trong mảng
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
            // Nếu không có gì để test, đảm bảo trạng thái equipped là false
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
    void HandleGravity()
    {
        if(characterController.isGrounded && playerlVelocityY < 0.0f)
        {
            // Nếu có, reset vận tốc rơi để nhân vật không bị tích tụ lực hấp dẫn khi đang đứng yên
            // Dùng một giá trị âm nhỏ để đảm bảo isGrounded luôn đúng
            playerlVelocityY = -2.0f;
        }
        else
        {
            playerlVelocityY += gravity * Time.deltaTime; // Thêm trọng lực nếu không chạm đất
        }
        Vector3 verticalMovement = new Vector3(0, playerlVelocityY, 0);
        // áp dụng di chuyển chiều dọc vào nhân vật
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
        // Hàm này sẽ được gọi bởi Animation Event khi animation tấn công kết thúc.
        // Nhiệm vụ của nó là yêu cầu state hiện tại (tức là AttackState) kiểm tra để chuyển sang state tiếp theo.
        currentState.CheckSwitchStates();
    }
}