using UnityEngine;

public class RagdollController : MonoBehaviour
{
    [SerializeField] private Health _health; // Ссылка на компонент здоровья
    [SerializeField] private Animator _animator; // Ссылка на аниматор персонажа
    [SerializeField] private CharacterController _characterController; // Если используется

    private Rigidbody[] _ragdollRigidbodies;
    private RigidbodyVelocityTracker[] _ragdollVelocityTracker;

    private void Awake()
    {
        if (_health == null)
            _health = GetComponent<Health>();

        if (_animator == null)
            _animator = GetComponentInChildren<Animator>();

        if (_characterController == null)
            _characterController = GetComponent<CharacterController>();

        _ragdollRigidbodies = GetComponentsInChildren<Rigidbody>();

        _ragdollVelocityTracker = new RigidbodyVelocityTracker[_ragdollRigidbodies.Length];

        for (int i = 0; i < _ragdollRigidbodies.Length; i++)
            _ragdollVelocityTracker[i] = new RigidbodyVelocityTracker(_ragdollRigidbodies[i].transform, _ragdollRigidbodies[i]);

        SetRagdollState(false);
    }

    private void OnEnable()
    {
        if (_health != null)
        {
            _health.OnDied += HandleDied;
        }
    }

    private void OnDisable()
    {
        if (_health != null)
        {
            _health.OnDied -= HandleDied;
        }
    }

    private void Update()
    {
        foreach(var velocityTracker  in _ragdollVelocityTracker)
            velocityTracker.Update(Time.deltaTime);
    }

    private void HandleDied()
    {

        // 2. Отключаем стандартное управление и аниматор
        if (_animator != null) _animator.enabled = false;
        if (_characterController != null) _characterController.enabled = false;

        // Отключаем этот скрипт или логику ввода, если нужно

        // 3. Включаем рагдолл
        SetRagdollState(true);

        // 4. Применяем накопленную скорость ко всем костям рагдолла
        foreach (var rb in _ragdollVelocityTracker)
        {
            rb.SetVelocityToRigidbody();
        }

        // Уничтожаем этот компонент или отписываемся, так как он больше не нужен
        enabled = false;
    }

    private void SetRagdollState(bool isActive)
    {
        // Если рагдолл активен, Rigidbody не кинематические. Если нет — кинематические (слушаются аниматором).
        foreach (var rb in _ragdollRigidbodies)
        {
            rb.isKinematic = !isActive;
        }


        if (_characterController != null)
        {
            _characterController.enabled = !isActive;
        }
    }
}