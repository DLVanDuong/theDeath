// PlayerStateMachine.cs
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(EquipmentManager))]
public class PlayerStateMachine : MonoBehaviour
{
    // === State Machine ===
    private PlayerBaseState _currentState;
    private PlayerStateFactory _states;

    // === Tham chiếu ===
    private CharacterController _characterController;
    private Animator _animator;
    private EquipmentManager _equipmentManager;

    // === Input ===
    private PlayerControls _inputActions;
    private Vector2 _currentMoveInput;
    public bool _isSprinting = false;
    public bool _isAttackPressed = false;

    // === Camera & Rotation ===
    [Header("Camera & Rotation")]
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private float rotationSpeed = 10f;

    // === Animator Hashes ===
    private readonly int _horizontalHash = Animator.StringToHash("Horizontal");
    private readonly int _verticalHash = Animator.StringToHash("Vertical");
    private readonly int _isSprintingHash = Animator.StringToHash("isSprinting");
    private readonly int _isEquippedHash = Animator.StringToHash("isEquipped");
    private readonly int _attackHash = Animator.StringToHash("Attack");

    // === Getters & Setters cho các State ===
    public PlayerBaseState CurrentState { get => _currentState; set => _currentState = value; }
    public Animator Animator { get => _animator; }
    public Vector2 CurrentMoveInput { get => _currentMoveInput; }
    public bool IsAttackPressed { get => _isAttackPressed; set => _isAttackPressed = value; }
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
        _characterController = GetComponent<CharacterController>();
        _animator = GetComponent<Animator>();
        _equipmentManager = GetComponent<EquipmentManager>();

        _inputActions = new PlayerControls();
        _inputActions.Player.Move.performed += ctx => _currentMoveInput = ctx.ReadValue<Vector2>();
        _inputActions.Player.Move.canceled += ctx => _currentMoveInput = Vector2.zero;
        _inputActions.Player.Sprint.performed += ctx => _isSprinting = true;
        _inputActions.Player.Sprint.canceled += ctx => _isSprinting = false;
        _inputActions.Player.Attack.performed += ctx => _isAttackPressed = true;

        if (cameraTransform == null) { cameraTransform = Camera.main.transform; }

        _states = new PlayerStateFactory(this);
        _currentState = _states.Idle();
        _currentState.EnterState();

        // SỬA LỖI: Thay đổi logic xử lý mảng testWeapon
        // 1. Kiểm tra xem mảng có tồn tại và có phần tử nào không
        if (testWeapon != null && testWeapon.Length > 0)
        {
            // 2. Dùng vòng lặp để trang bị TỪNG vật phẩm trong mảng
            foreach (EquipmentData item in testWeapon)
            {
                if (item != null)
                {
                    _equipmentManager.Equip(item);
                }
            }
        }
        else
        {
            // Nếu không có gì để test, đảm bảo trạng thái equipped là false
            _animator.SetBool(_isEquippedHash, false);
        }
    }

    void OnEnable() { _inputActions.Player.Enable(); }
    void OnDisable() { _inputActions.Player.Disable(); }

    void Update()
    {
        _currentState.UpdateState();
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
        _animator.SetFloat(_horizontalHash, _currentMoveInput.x);
        _animator.SetFloat(_verticalHash, _currentMoveInput.y);
        _animator.SetBool(_isSprintingHash, _isSprinting);
    }

    private void OnAnimatorMove()
    {
        if (_characterController != null)
        {
            _characterController.Move(_animator.deltaPosition);
        }
    }
}