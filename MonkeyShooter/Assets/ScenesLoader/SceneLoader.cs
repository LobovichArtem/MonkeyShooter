using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using System;


[DefaultExecutionOrder(-100)]
public class SceneLoader : MonoBehaviour
{
    private static SceneLoader _instance;
    public static SceneLoader Instance
    {
        get
        {            
            return _instance;
        }
    }

    private bool _isLoading = false;

    public float StartLoadingDelay { get; set; } = 0;
    public float EndLoadingDelay { get; set; } = 0;

    public event Action OnStartSceneLoading;
    public event Action OnAllScenesLoad;
    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
            Debug.Log("SceneLoader is Initialize");
        }
    }

    public void LoadScenesSequential(string mainScene, string[] additiveScenes = null, List<GameObject> prefabs = null)
    {
        if (_isLoading)
            return;
        StartCoroutine(LoadScenesAsyncSequential(mainScene, additiveScenes, prefabs));
        _isLoading = true;
    }

    public void Quit() => Application.Quit();

    private IEnumerator LoadScenesAsyncSequential(string mainScene, string[] additiveScenes, List<GameObject> prefabs)
    {
        if (string.IsNullOrEmpty(mainScene))// || (additiveScenes != null && additiveScenes.Length > 0 && additiveScenes.Any(s => string.IsNullOrEmpty(s))))
        {
            Debug.LogError("Invalid scene names");
            yield break;
        }

        OnStartSceneLoading?.Invoke();

        yield return new WaitForSeconds(StartLoadingDelay);
        // Загружаем основную сцену
        AsyncOperation mainLoad = SceneManager.LoadSceneAsync(mainScene, LoadSceneMode.Single);
        while (!mainLoad.isDone)
        {
            yield return null;
        }

        // Загружаем аддитивные сцены
        if (additiveScenes != null && additiveScenes.Length > 0)
        {
            foreach (string scene in additiveScenes)
            {
                AsyncOperation nextLoad = SceneManager.LoadSceneAsync(scene, LoadSceneMode.Additive);
                while (!nextLoad.isDone)
                {
                    yield return null;
                }
            }
        }

        //Загружаем список префабов
        if (prefabs != null)
        {
            foreach (var go in prefabs)
            {
                if(go != null)
                    Instantiate(go);
                yield return null;
            }
        }
        
        yield return new WaitForSeconds(EndLoadingDelay);
        _isLoading = false;
        OnAllScenesLoad?.Invoke();
    }


}

