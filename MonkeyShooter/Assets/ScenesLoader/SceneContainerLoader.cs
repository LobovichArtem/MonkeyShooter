using System.Collections.Generic;
using UnityEngine;

public class SceneContainerLoader : MonoBehaviour
{
    [SerializeField] private SceneConfigBase _sceneConfig;

    [ContextMenu("LoadScene")]
    public void LoadScene()
    {
        if (_sceneConfig == null)
        {
            Debug.LogError("Scene config is not assigned!");
            return;
        }

        LoadScene(_sceneConfig);
    }

    public void LoadScene(SceneConfigBase sceneConfig, List<GameObject> prefabs = null)
    {
        string mainSceneName = sceneConfig.GetMainSceneName();
        if (string.IsNullOrEmpty(mainSceneName))
        {
            Debug.LogError("Main scene name is empty!");
            return;
        }

        string[] additiveScenes = sceneConfig.GetAdditiveSceneNames();
        SceneLoader.Instance.LoadScenesSequential(mainSceneName, additiveScenes, prefabs);
    }

    public void Quit() => SceneLoader.Instance.Quit();
}