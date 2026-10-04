using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Scene Management/Additive Scenes Config")]
public class AdditiveScenesConfig : SingleSceneConfig
{
    [SerializeField] protected List<SceneType> _additiveScenes;

    public override string[] GetAdditiveSceneNames()
    {
        return _additiveScenes?.Select(s => s.SceneName).ToArray() ?? new string[0];
    }

    public override void ValidateAndUpdate()
    {
        base.ValidateAndUpdate();
        if (_additiveScenes != null)
        {
            foreach (var scene in _additiveScenes)
            {
                scene?.UpdateSceneNameForBuild();
            }
        }
    }
}