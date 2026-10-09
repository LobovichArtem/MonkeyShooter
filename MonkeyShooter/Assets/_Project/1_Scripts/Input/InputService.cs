using UnityEngine;
using UnityEngine.InputSystem;

public class InputService
{
    private readonly PlayerInput _input;

    public InputActionMap GeneralMap { get; private set; }
    public BaseInput<HumanInputData, HumanPacker> HumanInput {  get; private set; }
    public InputService()
    {
        _input = new PlayerInput();
        _input.Enable();

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
        return _input.asset.FindActionMap(mapName);
    }
}