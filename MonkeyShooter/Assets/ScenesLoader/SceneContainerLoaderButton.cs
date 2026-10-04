using UnityEngine.UI;

public class SceneContainerLoaderButton : SceneContainerLoader
{
    private void OnEnable()
    {
        if (TryGetComponent(out Button component))
            component.onClick.AddListener(LoadScene);
    }
    private void OnDisable()
    {
        if (TryGetComponent(out Button component))
            component.onClick.RemoveListener(LoadScene);
    }
}
