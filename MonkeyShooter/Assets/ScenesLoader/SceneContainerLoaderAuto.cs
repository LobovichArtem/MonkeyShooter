using UnityEngine;

public class SceneContainerLoaderAuto : SceneContainerLoader
{
    private void OnEnable()
    {
        LoadScene();
    }
    
}
