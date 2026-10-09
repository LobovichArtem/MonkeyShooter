
public class EntryPointGameplay : EntryPointBase
{
    public override void Initialize()
    {
        if (SceneLoader.Instance != null)
        {
            SceneLoader.Instance.OnAllScenesLoad -= Initialize;
        }

        var inputService = new InputService();
        ServiceLocator.Register(inputService);

        var windowService = new WindowsService();
        windowService.Initialize(true);
        ServiceLocator.Register(windowService);
    }

}