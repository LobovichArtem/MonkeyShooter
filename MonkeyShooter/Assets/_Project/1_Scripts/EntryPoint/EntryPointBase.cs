using UnityEngine;

[DefaultExecutionOrder(-10)]
public abstract class EntryPointBase : MonoBehaviour
{
    private void Awake()
    {
        if(SceneLoader.Instance != null)
        {
            SceneLoader.Instance.OnAllScenesLoad += Initialize;
        }
        else
        {
            Initialize();
        }
    }
    public abstract void Initialize();
}