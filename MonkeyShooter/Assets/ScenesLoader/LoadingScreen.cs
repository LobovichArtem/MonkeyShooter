using UnityEngine.Rendering;
using UnityEngine;
using System.Collections;

public class LoadingScreen : MonoBehaviour
{
    [field: SerializeField] public bool IsActive { get; private set; } = false;

    [SerializeField] private GameObject _loadingCanvas;
    [SerializeField] private Volume _loadingVolume;

    [field: SerializeField] public float FadeTime { get; private set; } = 1.0f;

    private Coroutine _fadeCoroutine;

    public void ToggleLoadingScreen(bool enable, float delay = 0f)
    {
        if (delay > 0f)
        {
            StartCoroutine(DelayedToggleLoadingScreen(enable, delay));
            return;
        }

        if (IsActive == enable) return;

        IsActive = enable;
        _loadingCanvas.SetActive(enable);

        if (_fadeCoroutine != null)
            StopCoroutine(_fadeCoroutine);

        float targetWeight = enable ? 1f : 0f;
        _fadeCoroutine = StartCoroutine(FadeVolume(targetWeight));
    }

    IEnumerator DelayedToggleLoadingScreen(bool enable, float delay)
    {
        yield return new WaitForSeconds(delay);
        ToggleLoadingScreen(enable);
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
}