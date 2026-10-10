using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

namespace AudioSourceManagment
{
    [CreateAssetMenu(fileName = "AudioClipLibrary", menuName = "AudioManagment/AudioClipLibrary")]
    public class AudioClipLibrary : ScriptableObject
    {
        [Serializable]
        private class AudioClipData
        {
            [field: SerializeField] internal string Key { get; set; }
            [field: SerializeField] internal List<AudioClip> _clips { get; set; } = new List<AudioClip>();
            public AudioClip Clip
            {
                get { return _clips[UnityEngine.Random.Range(0, _clips.Count)]; }
            }
        }
        public List<string> Keys => _clips.Select(clip => clip.Key).ToList();

        [SerializeField] private List<AudioClipData> _clips = new List<AudioClipData>();

        public AudioClip Get(string key)
        {
            var cclip = _clips.FirstOrDefault(clip => clip.Key.ToLower() == key.ToLower());
            return cclip.Clip;
        }


        [Space]
        [Header("Editor void for automatic implementation AudioClips and keys")]
        [SerializeField] private string _prefixToRemove = "SND_";
        [SerializeField] private List<AudioClip> _audioClips = new();

        [ContextMenu("CreateAudioClipList")]
        public void CreateAudioClipList()
        {
            if (_audioClips == null) return;

            // Группируем AudioClip по ключам (название без префикса)
            var groupedClips = _audioClips
                .Where(clip => clip != null && !string.IsNullOrEmpty(clip.name))
                .GroupBy(clip =>
                {
                    string clipName = clip.name;
                    // Убираем префикс если он есть
                    if (!string.IsNullOrEmpty(_prefixToRemove) && clipName.StartsWith(_prefixToRemove))
                    {
                        return clipName.Substring(_prefixToRemove.Length);
                    }
                    return clipName;
                })
                .ToList();

            // Получаем существующие ключи для проверки дубликатов
            var existingKeys = new HashSet<string>(_clips.Select(data => data.Key));

            int addedCount = 0;
            foreach (var group in groupedClips)
            {
                string key = group.Key;

                // Проверяем на дублирующийся ключ
                if (existingKeys.Contains(key))
                {
                    Debug.LogWarning($"Duplicate key '{key}' found. Skipping.");
                    continue;
                }

                // Добавляем новый элемент
                AudioClipData newData = new AudioClipData
                {
                    Key = key,
                    _clips = group.ToList()
                };
                _clips.Add(newData);
                existingKeys.Add(key); // Добавляем ключ в HashSet для последующих проверок
                addedCount++;
            }

            // Логируем результат
            Debug.Log($"Added {addedCount} new audio entries. Total: {_clips.Count} entries");
            _audioClips.Clear();
            // Сортируем по ключу для удобства (опционально)
            //audioClipList = audioClipList.OrderBy(data => data.Key).ToList();
        }
    }
}