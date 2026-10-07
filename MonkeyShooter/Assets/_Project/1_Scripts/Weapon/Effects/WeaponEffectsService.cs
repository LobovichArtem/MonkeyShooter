using UnityEngine;

public class WeaponEffectsService : MonoBehaviour
{
    [SerializeField] private PrimitiveType _markerType = PrimitiveType.Sphere;
    [SerializeField] private float _markerScale = 0.2f;
    [SerializeField] private float _destroyDelay = 5.0f;

    private void Awake()
    {
        ServiceLocator.Register(this);
    }

    private void OnDestroy()
    {
        ServiceLocator.Unregister<WeaponEffectsService>();
    }

    public void HandleShot(HitscanHitInfo hitInfo)
    {
        if (!hitInfo.DidHit)
            return;

        // Временный спавн тестовой сферы
        GameObject marker = GameObject.CreatePrimitive(_markerType);
        marker.transform.position = hitInfo.HitPoint;
        marker.transform.localScale = Vector3.one * _markerScale;

        // Отключаем коллайдер, чтобы тестовые сферы не мешали дальнейшим Raycast
        if (marker.TryGetComponent<Collider>(out var collider))
        {
            Destroy(collider);
        }

        Destroy(marker, _destroyDelay);
    }
}