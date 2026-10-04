using UnityEngine.Rendering;
using UnityEngine;
using System.Collections;

public class FadeScreen : MonoBehaviour
{
    private bool _isFadeStart = false;
    [SerializeField] private Volume _loadingVolume;
    [field: SerializeField] public float FadeTime { get; private set; } = 1.0f;

    [Header("Optional")]
    [SerializeField] private GameObject _loadingCanvas;

    private Coroutine _fadeVolumeCoroutine;
    private Coroutine _fadeCanvasCoroutine;

    private void Start()
    {
        _isFadeStart = true;
        EndFade(2);
    }

    public void StartFade() => StartFade(0);
    public void EndFade() => EndFade(0);
    public void StartFade(float delay = 0) => ToggleLoadingScreen(true, delay);

    public void EndFade(float delay = 0) => ToggleLoadingScreen(false, delay);

    private void ToggleLoadingScreen(bool enable, float delay = 0f)
    {
        if (delay > 0f)
        {
            StartCoroutine(DelayedToggleLoadingScreen(enable, delay));
            return;
        }
        if (_isFadeStart == enable) return;

        _isFadeStart = enable;
        float targetWeight = enable ? 1f : 0f;

        if (_loadingCanvas != null)
        {
            if (_loadingCanvas.TryGetComponent(out CanvasGroup group))
            {
                if (_fadeCanvasCoroutine != null)
                    StopCoroutine(_fadeCanvasCoroutine);
                _fadeCanvasCoroutine = StartCoroutine(FadeCanvas(group, targetWeight));
            }
            else
            {
                _loadingCanvas.SetActive(enable);
            }
        }
        if (_fadeVolumeCoroutine != null)
            StopCoroutine(_fadeVolumeCoroutine);

        _fadeVolumeCoroutine = StartCoroutine(FadeVolume(targetWeight));
    }

    IEnumerator DelayedToggleLoadingScreen(bool enable, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (enable)
            StartFade();
        else
            EndFade();
    }

    IEnumerator FadeVolume(float target)
    {
        float startWeight = _loadingVolume.weight;
        float elapsed = 0f;

        while (elapsed < FadeTime)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / FadeTime);
            _loadingVolume.weight = Mathf.Lerp(startWeight, target, t);
            yield return null;
        }

        _loadingVolume.weight = target;
    }

    IEnumerator FadeCanvas(CanvasGroup canvasGroup, float target)
    {
        float startWeight = canvasGroup.alpha;
        float elapsed = 0f;

        while (elapsed < FadeTime)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / FadeTime);
            canvasGroup.alpha = Mathf.Lerp(startWeight, target, t);
            yield return null;
        }

        canvasGroup.alpha = target;
    }
}