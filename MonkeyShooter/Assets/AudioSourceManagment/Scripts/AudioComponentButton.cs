using UnityEngine;
using UnityEngine.UI;

namespace AudioSourceManagment
{
    [RequireComponent (typeof (Button))]
    public class AudioComponentButton : AudioComponent
    {
        private void Awake()
        {
            GetComponent<Button>().onClick.AddListener(Play);
        }

        private void OnDestroy()
        {
            GetComponent<Button>().onClick.RemoveListener(Play);
        }
    }
}