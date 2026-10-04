using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[System.Serializable]
public class SceneType
{
#if UNITY_EDITOR
    [SerializeField] private SceneAsset _scene;
#endif    
    public string SceneName => _sceneName;
    [SerializeField] private string _sceneName;

    public void UpdateSceneNameForBuild()
    {
#if UNITY_EDITOR
        if (_scene != null)
        {
            _sceneName = _scene.name;
        }
#endif
    }
}