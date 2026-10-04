using UnityEngine;

public class EntryPointGlobal : EntryPointBase
{
    public override void Initialize()
    {
        //// 1. Сначала создаем данные сессии
        //_sessionSettings = new SessionSettings();
        //ServiceLocator.Register(_sessionSettings);
        //// 2. Создаем систему сохранения наборов ДТП на конкретных локациях (работает с файлами)
        //_saveSystem = new SaveSystemService();
        //ServiceLocator.Register(_saveSystem);
        //// 3. Создаем систему сохранения прогресса игрока
        //_playerProgressSaveService = new PlayerProgressSaveService();
        //ServiceLocator.Register(_playerProgressSaveService);
        //// 4. Создаем лончер для управления загрузкой сцен
        //_sceneLauncher = new SceneLauncher(_systemConfig);
        //ServiceLocator.Register(_sceneLauncher);
        ////Создаем единый инпут для управления контроллерами
        //_inputService = new InputService();
        //ServiceLocator.Register(_inputService);
        ////Создаем сервис настроек
        //var settings = new GameSettingsService(_gameSettingsConfig);
        //ServiceLocator.Register(settings);
        //_coroutineHelper = gameObject.AddComponent<CoroutineHelper>();
        //ServiceLocator.Register(_coroutineHelper);

        Debug.Log("[GlobalEntryPoint] All systems initialized and Registred in ServiceLocator.");
    }

    private void OnDestroy()
    {
        ServiceLocator.Clear();
    }
}