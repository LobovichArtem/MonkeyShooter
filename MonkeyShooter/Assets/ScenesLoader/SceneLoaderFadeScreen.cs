using UnityEngine;


public class SceneLoaderFadeScreen : MonoBehaviour
{
    [field: SerializeField] public FadeScreen LoadingScreen { get; set; }

    private void Start()
    {
        if(LoadingScreen == null)
            throw new System.NullReferenceException();

        SceneLoader.Instance.StartLoadingDelay = LoadingScreen.FadeTime;
        SceneLoader.Instance.OnStartSceneLoading += LoadingScreen.StartFade;
        SceneLoader.Instance.OnAllScenesLoad += LoadingScreen.EndFade;
    }

}

