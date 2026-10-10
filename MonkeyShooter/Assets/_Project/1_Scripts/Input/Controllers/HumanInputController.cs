using System;
using UnityEngine;

[RequireComponent(typeof(CharacterMover))]
public class HumanInputController : MonoBehaviour
{
    [SerializeField] private WeaponEffectsService _effectsService;

    [SerializeField] private Transform _cameraTransform;
    [SerializeField] private Transform _aimTransform;
    [SerializeField] private Transform _weaponContainer;
    [SerializeField] private MouseLookSettigns _mouseLookSettings;
    [SerializeField] private MovementConfig _config;

    private HumanInput _humanInput;
    private MouseLook _mouseLook;
    private MovementStateMashine _playerMovement;
    private WeaponHandler _weaponHandler;

    private float _jumpBufferTimer;

    private void Start()
    {
        if (_cameraTransform == null || _mouseLookSettings == null || _config == null)
        {
            throw new NullReferenceException();
        }
    
        InitializeInput();

        InitializeMouseLook();
        InitializeMovement();
        InitializeCombat();

    }

    private void InitializeMovement()
    {
        var mover = GetComponent<CharacterMover>();
        mover.Initialize(_config);
        _playerMovement = new MovementStateMashine(_config, mover);
        _playerMovement.InitializeStateMachine();
    }

    private void InitializeMouseLook()
    {
        _mouseLook = new MouseLook(_cameraTransform, _mouseLookSettings);
    }

    private void InitializeInput()
    {
        _humanInput = ServiceLocator.Get<InputService>().HumanInput as HumanInput;
        _humanInput.EnableControl();
    }
    private void InitializeCombat()
    {
        _weaponHandler = new WeaponHandler(_aimTransform, _weaponContainer);
    }

    private void Update()
    {
        if (_humanInput == null)
            return;

        HumanInputData inputData = _humanInput.GetInput();

        _mouseLook.UpdateLook(inputData.Look.x, inputData.Look.y);

        // 1. Формируем движение
        if (inputData.IsJump)
        {
            _jumpBufferTimer = Time.time + _config.JumpBufferTime;
        }

        Vector3 forward = _cameraTransform.forward;
        Vector3 right = _cameraTransform.right;
        forward.y = 0f;
        right.y = 0f;
        forward.Normalize();
        right.Normalize();

        Vector2 moveInput = new Vector2(inputData.Move.x, inputData.Move.y);
        moveInput = Vector3.ClampMagnitude(moveInput, 1f); // Ограничиваем длину единицей

        Vector3 moveDirection = forward * moveInput.y + right * moveInput.x;

        var movementInput = new MovementFrameInput
        {
            MoveDirection = moveDirection,
            LookDirection = forward,
            IsJumpRequested = Time.time < _jumpBufferTimer
        };

        // Отправляем ввод в сервис движения
        _playerMovement.ProcessInput(movementInput);

        var combatInput = new CombatFrameInput
        {
            IsFirePressed = inputData.IsAction,
            IsFireHeld = inputData.IsActionHeld,
            IsReloadRequested = inputData.IsReloadPressed
        };

        _weaponHandler.ProcessInput(combatInput);
    }

    public void SelectWeapon(WeaponData weaponData)
    {
        if(weaponData == null)
            return;
        _weaponHandler.EquipWeapon(weaponData);
    }
}