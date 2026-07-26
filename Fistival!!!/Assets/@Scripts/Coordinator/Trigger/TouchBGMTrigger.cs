using Assets._Scripts.Coordinator;
using UnityEngine;

namespace Coordinator.Trigger
{
    public class TouchBGMTrigger : BGMTriggerBase, IBasicInitializer
    {
        [SerializeField]
        private bool _playOncePerLife;
        private bool _isPlayed;
        private void Start()
        {
            var trigger = GetComponent<TouchTrigger>();
            trigger.OnTriggerActivated -= Play;
            trigger.OnTriggerActivated += Play;
            _isPlayed = false;
        }

        public void Init()
        {
            _isPlayed = false;
        }

        private void Play(GameObject triggered)
        {
            if(_isPlayed && _playOncePerLife)
            {
                return;
            }
            _isPlayed = true;
            Play();
        }
    }
}