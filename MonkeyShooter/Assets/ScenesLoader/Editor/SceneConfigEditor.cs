#if UNITY_EDITOR
using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SceneConfigBase), true)]
public class SceneConfigEditor : Editor
{
    private SceneConfigBase _config;

    private void OnEnable()
    {
        _config = (SceneConfigBase)target;
    }

    public override void OnInspectorGUI()
    {
        var previousMainSceneName = _config.GetMainSceneName();
        var previousAdditiveScenes = _config.GetAdditiveSceneNames()?.ToArray();

        DrawDefaultInspector();

        bool hasChanges = false;

        if (_config.GetMainSceneName() != previousMainSceneName)
        {
            hasChanges = true;
        }

        var currentAdditiveScenes = _config.GetAdditiveSceneNames();
        if (currentAdditiveScenes != null &&
            !(currentAdditiveScenes.SequenceEqual(previousAdditiveScenes ?? new string[0])))
        {
            hasChanges = true;
        }

        if (hasChanges)
        {
            _config.ValidateAndUpdate();
            EditorUtility.SetDirty(_config);
        }
    }
}
#endif