using System;
using System.Collections.Generic;
using UnityEngine;

public static class ServiceLocator
{
    private static readonly Dictionary<Type, object> _services = new Dictionary<Type, object>();

    /// <summary>
    /// Регистрирует сервис. Если сервис такого типа уже есть, он будет перезаписан.
    /// </summary>
    public static void Register<T>(T service)
    {
        var type = typeof(T);
        if (_services.ContainsKey(type))
        {
            _services[type] = service;
        }
        else
        {
            _services.Add(type, service);
        }
        //Debug.Log($"Зарегистрирован сервис [{type}]. Всего сервисов {_services.Count}");
    }

    /// <summary>
    /// Возвращает сервис заданного типа.
    /// </summary>
    public static T Get<T>()
    {
        var type = typeof(T);
        if (_services.TryGetValue(type, out var service))
        {
            return (T)service;
        }

        Debug.LogError($"[ServiceLocator] Сервис типа {type} не зарегистрирован!");
        return default;
    }

    public static bool TryGet<T>(out T component)
    {
        var type = typeof(T);
        if (_services.TryGetValue(type, out var service))
        {
            component = (T)service;
            return true;
        }
        component = default;
        return false;
    }

    public static List<T> GetAll<T>()
    {
        var targetType = typeof(T);
        var result = new List<T>();

        foreach (var service in _services)
        {
            // Проверяем, можно ли назначить тип сервиса переменной типа T
            if (targetType.IsAssignableFrom(service.Key))
            {
                result.Add((T)service.Value);
            }
        }

        if (result.Count == 0)
        {
            Debug.LogWarning($"[ServiceLocator] Не найдено ни одного сервиса, наследуемого от {targetType}!");
        }

        return result;
    }

    /// <summary>
    /// Удаляет конкретный сервис из реестра.
    /// </summary>
    public static void Unregister<T>()
    {
        _services.Remove(typeof(T));
        //Debug.Log($"Удален сервис [{typeof(T)}]. Всего сервисов {_services.Count}");
    }

    /// <summary>
    /// ПОЛНАЯ ОЧИСТКА. Вызывать при смене сцены в SceneServices.OnDestroy.
    /// </summary>
    public static void Clear()
    {
        _services.Clear();
        Debug.Log($"Все сервисы очищены. Всего сервисов {_services.Count}");
    }
}