using UnityEngine;

namespace AudioSourceManagment
{
    [DefaultExecutionOrder(-1)]
    public class AudioSourceManager : MonoBehaviour
    {
        public static AudioSourceManager Instance
        {
            get
            {
                return _instance;
            }
        }
        private static AudioSourceManager _instance;

        private AudioSourcePool _pool;

        private AudioSourceSettings _defaultSettings;

        private AudioClipLibrary[] _audioClipLibraries;

        [SerializeField] private int _poolCount = 5;

        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
                _instance.LoadAudioClipLibrary();
                var prefab = new GameObject("AudioSource").AddComponent<AudioSource>();
                prefab.transform.SetParent(transform);
                prefab.playOnAwake = false;
                _instance.SetSourceSettings(prefab, null);
                _instance._pool = new AudioSourcePool(prefab, _poolCount);
                DontDestroyOnLoad(this);
            }
            else
            {
                if (_instance != this)
                {
                    Destroy(this.gameObject);
                }
            }
        }

        public void OnDestroy()
        {
            if (_instance != this)
                return;
            Instance._pool.Dispose();
            _instance = null;
        }

        public AudioClip GetAudioClipToKey(string key)
        {
            foreach (var library in _audioClipLibraries)
                return library.Get(key);
            throw new System.NullReferenceException($"Do not search audio clip to key. Add audio clip to key {key}");
        }

        public void Play(string key)
        {
            var clip = GetAudioClipToKey(key);
            if (clip != null)
                Play(clip);
        }

        public void Play(string key, Vector3 position)
        {
            var clip = GetAudioClipToKey(key);
            if (clip != null)
                Play(clip, position);
        }

        public void Play(AudioClip clip)
        {
            var source = _pool.Get();
            SetSourceSettings(source, _defaultSettings);
            source.PlayOneShot(clip);
        }

        //public void Play(AudioClip clip, AudioSourceSettings settings)
        //{
        //    var source = _pool.Get();
        //    SetSourceSettings(source, settings);

        //    source.PlayOneShot(clip);
        //}

        //public void Play(AudioClip clip, Vector3 position)
        //{
        //    var source = _pool.Get();
        //    SetSourceSettings(source, _defaultSettings);
        //    source.transform.position = position;

        //    source.PlayOneShot(clip);
        //}

        public void Play(AudioClip clip, Vector3 position, AudioSourceSettings settings = null)
        {
            var source = _pool.Get();
            SetSourceSettings(source, settings);
            source.transform.position = position;

            source.PlayOneShot(clip);
        }

        private void LoadAudioClipLibrary()
        {
            var s = Resources.LoadAll("", typeof(AudioClipLibrary));
            _audioClipLibraries = new AudioClipLibrary[s.Length];
            s.CopyTo(_audioClipLibraries, 0);
            if (_audioClipLibraries.Length == 0)
                throw new System.IndexOutOfRangeException("Audio Clip files not found");

            _defaultSettings = Resources.Load<AudioSourceSettings>("DeffaultAudioSourceSettings");

            Debug.Log($"AudioClipsLibrary: load {_audioClipLibraries.Length} files");
        }

        private void SetSourceSettings(AudioSource source, AudioSourceSettings settings)
        {
            if (settings == null)
                settings = _defaultSettings;

            source.volume = settings.Volume;
            source.loop = settings.Loop;
            source.priority = settings.Priority;
            source.spatialBlend = settings.SpatialBlend;
            source.minDistance = settings.MinMaxDistance.x;
            source.maxDistance = settings.MinMaxDistance.y;
        }


    }
}
