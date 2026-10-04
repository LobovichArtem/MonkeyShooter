using UnityEngine;

public abstract class SceneConfigBase : ScriptableObject
{
    public abstract string GetMainSceneName();
    public virtual string[] GetAdditiveSceneNames()
    {
        return new string[0];
    }
    public abstract void ValidateAndUpdate();
}