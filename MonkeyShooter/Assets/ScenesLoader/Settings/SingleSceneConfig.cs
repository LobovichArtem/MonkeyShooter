using UnityEngine;

[CreateAssetMenu(menuName = "Scene Management/Single Scene Config")]
public class SingleSceneConfig : SceneConfigBase
{
    [SerializeField] protected SceneType _mainScene;

    public override string GetMainSceneName()
    {
        return _mainScene?.SceneName ?? "";
    }

    public override void ValidateAndUpdate()
    {
        _mainScene?.UpdateSceneNameForBuild();
    }
}