using UnityEngine;

public class HumanInputController : MonoBehaviour
{
    [SerializeField] private MovementConfig _config;

    [SerializeField] private CharacterMover _mover;
    [SerializeField] private Transform _cameraTransform;

    [SerializeField] private MouseLookSettigns _mouseLookSettings;

    private MovementStateMachine _stateMachine;
    private HumanInput _humanInput;
    private MouseLook _mouseLook;

    private float _jumpBufferTimer;

    public void Construct(HumanInput humanInput)
    {
        _humanInput = humanInput;
        _humanInput.EnableControl();
    }

    private void Awake()
    {
        if (_cameraTransform == null && Camera.main != null)
        {
            _cameraTransform = Camera.main.transform;
        }
        InitializeStateMachine();
    }

    private void Start()
    {
        var input = new HumanInput();
        Construct(input);

        _mouseLook = new MouseLook(_cameraTransform, _mouseLookSettings);
    }

    private void InitializeStateMachine()
    {
        _stateMachine = new MovementStateMachine();

        var airborneState = new AirborneState(_mover, _config, _stateMachine);
        var groundedState = new GroundedState(_mover, _config, _stateMachine, airborneState);
        var wallLatchState = new WallLatchState(_mover, _config, _stateMachine);
        airborneState.SetGroundedState(groundedState);
        airborneState.SetWallLatchState(wallLatchState);
        wallLatchState.SetStates(airborneState, groundedState);
        _stateMachine.Initialize(groundedState);
    }

    private void Update()
    {
        if (_humanInput == null || !_humanInput.IsActive)
            return;

        HumanInputData inputData = _humanInput.GetInput();

        _mouseLook.UpdateLook(inputData.Look.x, inputData.Look.y);

        // 1. Обновление таймера буфера прыжка
        if (inputData.IsJump)
        {
            _jumpBufferTimer = Time.time + _config.JumpBufferTime;
        }

        // 2. Расчет направления движения относительно поворота камеры
        Vector3 forward = _cameraTransform.forward;
        Vector3 right = _cameraTransform.right;

        forward.y = 0f;
        right.y = 0f;

        forward.Normalize();
        right.Normalize();

        Vector3 moveDirection = forward * inputData.Move.y + right * inputData.Move.x;

        // 3. Упаковка в кадровую структуру (флаг активен, пока запущен таймер буфера)
        var frameInput = new MovementFrameInput
        {
            MoveDirection = moveDirection,
            LookDirection = forward,
            IsJumpRequested = Time.time < _jumpBufferTimer
        };

        // 4. Обновление стейт-машины
        _stateMachine.Update(frameInput);
    }

}