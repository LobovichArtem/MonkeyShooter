using UnityEngine;

public class WeaponView : MonoBehaviour
{
    [field: SerializeField] public Transform FirePoint { get; private set; }
    [SerializeField] private Animator _animator;
    [SerializeField] private ParticleSystem _muzzleFlash;
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip _shotSound;

    // Имена параметров в Аниматоре
    private static readonly int ShootHash = Animator.StringToHash("Shoot");
    private static readonly int ReloadHash = Animator.StringToHash("Reload");

    public void PlayShootEffects()
    {
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