using UnityEngine;
using UnityEngine.InputSystem;

public class InputService
{
    private readonly PlayerInput _actions;

    [field: SerializeField]
    public InputActionMap CurrentMap { get; private set; }

    public InputActionMap GeneralMap { get; private set; }
    public BaseInput<HumanInputData, HumanPacker> HumanInput {  get; private set; }
    public InputService()
    {
        _actions = new PlayerInput();
        _actions.Enable();

        GeneralMap = GetMap("General");
        GeneralMap.Enable();

        var human = GetMap("Human");
        HumanPacker paker = new HumanPacker();
        paker.Initialize(human);
        HumanInput = new HumanInput(paker);
    }

    //public void SwitchMap(string mapName)
    //{
    //    // Выключаем только текущую карту, если она есть
    //    CurrentMap?.Disable();

    //    CurrentMap = GetMap(mapName);
    //    if (CurrentMap == null)
    //    {
    //        Debug.LogError($"[InputService] Map '{mapName}' not found!");
    //        return;
    //    }

    //    CurrentMap.Enable();
    //    Debug.Log($"[InputService] Switched to: {CurrentMap.name}");
    //}

    // Метод для получения карты без её активации (нужен для инициализации пакеров)
    private InputActionMap GetMap(string mapName)
    {
        return _actions.asset.FindActionMap(mapName);
    }
}