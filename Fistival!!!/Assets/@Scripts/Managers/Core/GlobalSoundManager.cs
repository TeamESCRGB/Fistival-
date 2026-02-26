using UnityEngine;
using Defines;
using System.Collections.Generic;

namespace Manager.Core
{
    public class GlobalSoundManager
    {
        private Dictionary<string,AudioClip> _cachedSounds = new Dictionary<string,AudioClip>(32);
        private AudioSource[] _channels = null;
        private GameObject _soundRoot = null;

        public void Init()
        {
            if (_soundRoot == null)
            {
                _soundRoot = GameObject.Find("@GlobalSoundRoot");
                if (_soundRoot == null)
                {
                    _soundRoot = new GameObject { name = "@GlobalSoundRoot" };
                    Object.DontDestroyOnLoad(_soundRoot);

                    string[] soundTypeNames = System.Enum.GetNames(typeof(SoundChannel));
                    int channelCnt = (int)SoundChannelInfo.CHANNEL_CNT;
                    _channels = new AudioSource[channelCnt];
                    for(int i = 0; i < channelCnt; i++)
                    {
                        GameObject go = new GameObject(soundTypeNames[i]);
                        go.transform.parent = _soundRoot.transform;
                        _channels[i] = go.AddComponent<AudioSource>();
                    }
                }
            }
        }

        public void Play(SoundChannel channel, string key, bool loop, float volume = 1.0f, float pitch = 1.0f)
        {
            if (channel < SoundChannel.BGM_0 || (int)channel >= (int)SoundChannelInfo.CHANNEL_CNT)
            {
#if UNITY_EDITOR
                Debug.LogWarning("wrong channel number");
#endif  
                return;
            }

            AudioSource audioSource = _channels[(int)channel];
            AudioClip clip = LoadAudioClip(key);

            if (clip == null)
            {
                return;
            }

            audioSource.pitch = pitch;
            audioSource.volume = volume;

            if (channel < SoundChannel.EFFECT_0)
            {
                audioSource.Stop();
                audioSource.loop = loop;
                audioSource.clip = clip;
                audioSource.Play();
            }
            else
            {
                Debug.Log("eff");
                audioSource.PlayOneShot(clip);
            }
        }

        public void PlayBGMInIdleChannel(SoundChannelType type, string key, bool loop, float volume = 1.0f, float pitch = 1.0f)
        {
            SoundChannel channel = SoundChannel.UNKNOWN;
            int channelIdxStart = (int)type;
            int channelCnt = GetChannelCnt(type);

            for (int i = channelIdxStart, end = channelIdxStart + channelCnt; i < end; i++)
            {
                if (_channels[i].isPlaying == false)
                {
                    channel = (SoundChannel)i;
                }
            }

            Play(channel, key, loop, volume, pitch);
        }

        public void StopAll()
        {
            foreach (var source in _channels)
            {
                source.Stop();
            }
        }

        public void StopAllOf(SoundChannelType type)
        {
            int channelIdxStart = (int)type;
            int channelCnt = GetChannelCnt(type);

            for(int i=channelIdxStart, end=channelIdxStart+channelCnt; i < end;i++)
            {
                _channels[i].Stop();
            }
        }

        public void Clear()
        {
            StopAll();
            _cachedSounds.Clear();
        }


        private AudioClip LoadAudioClip(string audioClipName)
        {
            AudioClip clip;
            if (_cachedSounds.TryGetValue(audioClipName, out clip))
            {
                return clip;
            }

            clip = Managers.Instance.ResourceManager.Load<AudioClip>(audioClipName);
            if (clip == null)
            {
                _cachedSounds.Add(audioClipName, clip);
            }
            return clip;
        }

        public int GetChannelCnt(SoundChannelType type)
        {
            switch (type)
            {
                case SoundChannelType.BGM:
                    return (int)SoundChannelInfo.BGM_CHANNEL_CNT;
                case SoundChannelType.SUB_BGM:
                    return (int)SoundChannelInfo.SUB_BGM_CHANNEL_CNT;
                case SoundChannelType.EFFECT:
                    return (int)SoundChannelInfo.EFFECT_CHANNEL_CNT;
            }
            return -1;
        }
    }
}