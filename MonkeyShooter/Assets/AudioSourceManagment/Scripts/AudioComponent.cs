using UnityEngine;

namespace AudioSourceManagment
{
    public class AudioComponent : MonoBehaviour
    {
        [SerializeField] private AudioKey _key;

        [SerializeField] private AudioSourceSettings _audioSourceSettings;

        [SerializeField] private bool _playOnEnable;
        [SerializeField] private bool _ignoreFirstEnable;
        private bool _firstEnable = false;

        [SerializeField] private bool _playOnDisable;

        [SerializeField] private bool _playOnCollisionEnter;

        private Vector3 _soundPosition;

        [ContextMenu("Play")]
        public void Play()
        {
            if (string.IsNullOrEmpty(_key.Key))
                return;
            var clip = AudioSourceManager.Instance.GetAudioClipToKey(_key);
            AudioSourceManager.Instance.Play(clip, _soundPosition, _audioSourceSettings);
        }

        private void OnEnable()
        {
            if (_playOnEnable)
            {
                if(_ignoreFirstEnable &&  _firstEnable == false)
                {
                    _firstEnable = true;
                    return;
                }
                _soundPosition = transform.position;
                Play();
            }
        }

        private void OnDisable()
        {
            if (_playOnDisable)
            {
                _soundPosition = transform.position;
                Play();
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (_playOnCollisionEnter)
            {
                _soundPosition = collision.contacts[0].point;
                Play();
            }
        }
    }
}
