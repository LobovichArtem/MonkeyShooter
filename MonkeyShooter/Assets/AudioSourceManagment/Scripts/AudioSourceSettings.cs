using UnityEngine;

namespace AudioSourceManagment
{
    [CreateAssetMenu(fileName = "AudioSourceSettingsDeffault", menuName = "AudioManagment/AudioSourceSettings")]
    public class AudioSourceSettings : ScriptableObject
    {
        public float Volume = 0.8f;
        public bool Loop = false;
        public int Priority = 128;
        public int SpatialBlend = 0;
        public Vector2 MinMaxDistance = new Vector2(1, 500);
    }
}
