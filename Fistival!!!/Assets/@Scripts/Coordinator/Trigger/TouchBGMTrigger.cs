using UnityEngine;

namespace Coordinator.Trigger
{
    public class TouchBGMTrigger : BGMTriggerBase
    {
        private void Start()
        {
            var trigger = GetComponent<TouchTrigger>();
            trigger.OnTriggerActivated -= Play;
            trigger.OnTriggerActivated += Play;
        }

        private void Play(GameObject triggered)
        {
            Play();
        }
    }
}