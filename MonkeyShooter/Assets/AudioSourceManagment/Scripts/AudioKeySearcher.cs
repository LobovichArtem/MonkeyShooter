#if UNITY_EDITOR
using UnityEditor;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using AudioSourceManagment; // Твое пространство имен для AudioClipLibrary

public static class AudioKeySearcher
{
    private static string[] _cachedKeys;
    private static double _lastRefresh;

    public static string[] GetAllKeys()
    {
        if (_cachedKeys != null && EditorApplication.timeSinceStartup - _lastRefresh < 10.0)
            return _cachedKeys;

        // Загружаем только аудио-библиотеки
        var libraries = Resources.LoadAll<AudioClipLibrary>("");
        var keys = new HashSet<string>();

        foreach (var library in libraries)
        {
            if (library == null || library.Keys == null) continue;
            foreach (var key in library.Keys)
            {
                if (!string.IsNullOrEmpty(key)) keys.Add(key);
            }
        }

        _cachedKeys = keys.OrderBy(k => k).ToArray();
        _lastRefresh = EditorApplication.timeSinceStartup;
        return _cachedKeys;
    }
}
#endif