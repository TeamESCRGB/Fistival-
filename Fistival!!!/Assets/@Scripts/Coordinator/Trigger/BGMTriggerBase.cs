using Defines;
using Manager;
using UnityEngine;

namespace Coordinator.Trigger
{
    public abstract class BGMTriggerBase : MonoBehaviour
    {
        [SerializeField]
        protected SoundChannel _channel;
        [SerializeField]
        protected string _soundKey;
        protected void Play()
        {
            Managers.Instance.GlobalSoundManager.Play(_channel, _soundKey,true,Managers.Instance.GameManager.BGMVolume);
        }
    }
}
