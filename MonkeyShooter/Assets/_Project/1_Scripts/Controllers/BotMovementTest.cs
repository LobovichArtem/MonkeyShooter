using UnityEngine;

[RequireComponent(typeof(CharacterMover))]
public class BotMovementTest : MonoBehaviour
{
    [SerializeField] private MovementConfig _config;
    [SerializeField] private float _runDuration = 2f; // Сколько секунд бежать в одну сторону

    private CharacterMover _mover;
    private MovementStateMashine _playerMovement;

    private float _timer;

    private void Start()
    {
        _mover = GetComponent<CharacterMover>();
        GetComponent<Health>().OnDied += () => _config = null;

        if (_config != null)
        {
            _mover.Initialize(_config);
        }

        // Если используешь стейт-машину, инициализируем её
        if (_config != null)
        {
            _playerMovement = new MovementStateMashine(_config, _mover);
            _playerMovement.InitializeStateMachine();
        }

        _timer = _runDuration;
    }

    private void Update()
    {
        if (_config == null) return;

        _timer -= Time.deltaTime;

        if (_timer <= 0f)
        {
            // Меняем направление на противоположное (разворот на 180 градусов)
            transform.eulerAngles += Vector3.up* 180f; // Мгновенный разворот меша/трансформа на 180
            _timer = _runDuration; // Сбрасываем таймер
        }

        // Формируем ввод для движения вперед относительно текущего взгляда бота
        Vector3 moveDirection = transform.forward;

        // Если используем стейт-машину
        if (_playerMovement != null)
        {
            var frameInput = new MovementFrameInput
            {
                MoveDirection = moveDirection,
                LookDirection = transform.forward,
                IsJumpRequested = false
            };
            _playerMovement.ProcessInput(frameInput);
        }

    }
}