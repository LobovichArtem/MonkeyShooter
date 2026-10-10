using UnityEngine;

public class VelocityTracker
{
    protected readonly Transform _targetTransform;
    protected Vector3 _previousPosition;
    protected Vector3 _currentVelocity;

    public Vector3 CurrentVelocity => _currentVelocity;

    public VelocityTracker(Transform targetTransform)
    {
        _targetTransform = targetTransform;
        _previousPosition = _targetTransform.position;
    }

    public void Update(float deltaTime)
    {
        if (deltaTime <= 0f) return;

        Vector3 currentPosition = _targetTransform.position;
        _currentVelocity = (currentPosition - _previousPosition) / deltaTime;
        _previousPosition = currentPosition;
    }


}

public class RigidbodyVelocityTracker : VelocityTracker
{
    protected readonly Rigidbody _targetRigidbody;
    public RigidbodyVelocityTracker(Transform targetTransform, Rigidbody targetRigidbody) : base(targetTransform)
    {
        _targetRigidbody = targetRigidbody;
    }
    public void SetVelocityToRigidbody()
    {
        _targetRigidbody.linearVelocity = _currentVelocity;
    }
}