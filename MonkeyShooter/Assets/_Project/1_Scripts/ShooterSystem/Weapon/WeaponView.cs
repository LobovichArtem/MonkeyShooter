using UnityEngine;

public class WeaponView : MonoBehaviour
{
    [field: SerializeField] public WeaponData WeaponData {  get; private set; }
    [field: SerializeField] public Transform FirePoint { get; private set; }

    [Header("Procedural Recoil Container")]
    [SerializeField] private Transform _recoilContainer; // Ссылка на дочерний объект с 3D-моделью

    [Header("Visual & Sound Effects")]
    [SerializeField] private Animator _animator;
    [SerializeField] private ParticleSystem _muzzleFlash;
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip _shotSound;

    private ProceduralWeaponRecoil _proceduralRecoil;

    private static readonly int ShootHash = Animator.StringToHash("Shoot");
    private static readonly int ReloadHash = Animator.StringToHash("Reload");

    private void Awake()
    {
        Transform targetTransform = _recoilContainer != null ? _recoilContainer : transform;
        _proceduralRecoil = new ProceduralWeaponRecoil(targetTransform);
    }

    public void Init(WeaponKickConfigSO kickConfig)
    {
        if (kickConfig != null)
        {
            _proceduralRecoil?.SetConfig(kickConfig);
        }
    }

    private void Update()
    {
        _proceduralRecoil?.Update(Time.deltaTime);
    }

    public void PlayShootEffects()
    {
        _proceduralRecoil?.PlayRecoil();

        if (_muzzleFlash != null)
            _muzzleFlash.Play();

        if (_audioSource != null && _shotSound != null)
            _audioSource.PlayOneShot(_shotSound);

        if (_animator != null)
            _animator.SetTrigger(ShootHash);
    }

    public void PlayReloadAnimation()
    {
        if (_animator != null)
            _animator.SetTrigger(ReloadHash);
    }
}