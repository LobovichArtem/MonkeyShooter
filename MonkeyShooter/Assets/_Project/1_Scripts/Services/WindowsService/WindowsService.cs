using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public sealed class WindowsService
{
    private bool _lookCursor = false;
    // Храним только логику закрытия
    private readonly List<Action> _closeActions = new List<Action>();

    public bool CursorLock => Cursor.lockState == CursorLockMode.Locked;

    // Событие для "свободного" нажатия ESC
    public event Action OnEscapeUnhandled;


    public void Initialize(bool cursorLock)
    {
        _lookCursor = cursorLock;
        var globalInput = ServiceLocator.Get<InputService>().GeneralMap;

        globalInput.FindAction("Cancel").performed += OnCancelPerformed;
        UpdateCursorState();
    }
    

    /// <summary>
    /// Регистрация действия закрытия. Вызывать при открытии любого окна.
    /// </summary>
    public void OpenWindow(Action onClose)
    {
        if (onClose == null) 
            return;

        _closeActions.Add(onClose);
        UpdateCursorState();
    }

    /// <summary>
    /// Вызывать при закрытии окна не через кнопку (например крестик UI), чтобы удалить из очереди на закрытие старый экшен.
    /// </summary>
    public void CloseWindow(Action onClose)
    {
        if (_closeActions.Contains(onClose))
        {
            _closeActions.Remove(onClose);
            UpdateCursorState();
        }
    }

    private void CloseLastWindow()
    {
        if(_closeActions.Count == 0)
        {
            UpdateCursorState();
            return;
        }
        int lastIndex = _closeActions.Count - 1;
        Action lastAction = _closeActions[lastIndex];

        // Удаляем из списка ПЕРЕД вызовом, чтобы избежать рекурсии, 
        // если внутри экшена снова вызовется проверка состояния
        _closeActions.RemoveAt(lastIndex);

        lastAction?.Invoke();
        UpdateCursorState();
    }

    private void UpdateCursorState()
    {
        if (_lookCursor == false)
            return;
        bool hasActiveActions = _closeActions.Count > 0;
        Cursor.lockState = hasActiveActions ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = hasActiveActions;

    }

    private void OnCancelPerformed(InputAction.CallbackContext context)
    {
        if (_closeActions.Count > 0)
            CloseLastWindow();
        else
            OnEscapeUnhandled?.Invoke();
    }
}