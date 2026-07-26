using UnityEngine;
using Defines;
using System.Collections.Generic;

namespace Manager.Core
{
    public class GlobalSoundManager
    {
        private Dictionary<string,AudioClip> _cachedSounds = new Dictionary<string,AudioClip>(32);
        private double[] _pauseTimes = null;
        private double[] _pauseStartedTimes = null;
        private (double startTime, AudioSource source)[] _channels = null;
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
                    _channels = new (double, AudioSource)[channelCnt];
                    _pauseTimes = new double[channelCnt];
                    _pauseStartedTimes = new double[channelCnt];
                    for(int i = 0; i < channelCnt; i++)
                    {
                        GameObject go = new GameObject(soundTypeNames[i]);
                        go.transform.parent = _soundRoot.transform;
                        _channels[i].source = go.AddComponent<AudioSource>();
                    }
                }
            }
            AudioListener.pause = false;
        }

        /// <summary>
        /// volume값은 더이상 쓰이지 않음
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="key"></param>
        /// <param name="loop"></param>
        /// <param name="volume"></param>
        /// <param name="pitch"></param>
        public void Play(SoundChannel channel, string key, bool loop, float volume = 1.0f, float pitch = 1.0f)
        {
            AudioSource audioSource = GetSource(channel);

            if(audioSource == null)
            {
                return;
            }


            AudioClip clip = LoadAudioClip(key);

            if (clip == null)
            {
                return;
            }

            audioSource.pitch = pitch;
            //audioSource.volume = volume;
            _pauseTimes[(int)channel] = 0;
            if (channel < SoundChannel.EFFECT_0)
            {
                audioSource.Stop();
                audioSource.loop = loop;
                audioSource.clip = clip;
                audioSource.Play();
                _channels[(int)channel] = (AudioSettings.dspTime, audioSource);
            }
            else
            {
                Debug.Log("eff");
                audioSource.PlayOneShot(clip);
            }
        }

        public void Pause(SoundChannel channel)
        {
            var source = GetSource(channel);
            if(source == null || source.isPlaying == false)
            {
                return;
            }
            _pauseStartedTimes[(int)channel] = AudioSettings.dspTime;
            source.Pause();
            
        }

        public void SetChannelVolumAt(SoundChannelType type, float volume)
        {
            switch(type)
            {
                case SoundChannelType.BGM:
                    GetSource(SoundChannel.BGM_0).volume = volume;
                    GetSource(SoundChannel.BGM_1).volume = volume;
                    break;
                case SoundChannelType.SUB_BGM:
                    GetSource(SoundChannel.SUB_BGM_0).volume = volume;
                    GetSource(SoundChannel.SUB_BGM_1).volume = volume;
                    break;
                case SoundChannelType.EFFECT:
                    GetSource(SoundChannel.EFFECT_0).volume = volume;
                    GetSource(SoundChannel.EFFECT_1).volume = volume;
                    break;
            }
        }

        public void UnPause(SoundChannel channel)
        {
            var source = GetSource(channel);
            if (source == null || source.isPlaying)
            {
                return;
            }

            source.UnPause();
            _pauseTimes[(int)channel] += AudioSettings.dspTime - _pauseStartedTimes[(int)channel];
        }

        public void PauseAll()
        {
            foreach(int a in System.Enum.GetValues(typeof(SoundChannel)))
            {
                if(a<0)
                {
                    return;
                }
                Pause((SoundChannel)a);
            }
            AudioListener.pause = true;
        }

        public void UnPauseAll()
        {
            AudioListener.pause = false;
            foreach (int a in System.Enum.GetValues(typeof(SoundChannel)))
            {
                if (a < 0)
                {
                    return;
                }
                UnPause((SoundChannel)a);
            }
        }

        public double GetDSPTime(SoundChannel channel)
        {
            var source = GetSource(channel);

            if(source == null)
            {
                return -1;
            }

            if(source.isPlaying)
            {
                return AudioSettings.dspTime - _channels[(int)channel].startTime - _pauseTimes[(int)channel];
            }
            else if(source.time > 0)//
            {
                return _pauseStartedTimes[(int)channel] - _channels[(int)channel].startTime - _pauseTimes[(int)channel];
            }

            return 0;
        }

        public bool IsPlaying(SoundChannel channel)
        {
            return GetSource(channel).isPlaying;
        }

        public float GetSoundTime(SoundChannel channel)
        {
            return GetSource(channel).time;
        }

        
        private AudioSource GetSource(SoundChannel channel)
        {
            if (channel < SoundChannel.BGM_0 || (int)channel >= (int)SoundChannelInfo.CHANNEL_CNT)
            {
#if UNITY_EDITOR
                Debug.LogWarning("wrong channel number");
#endif  
                return null;
            }
            return _channels[(int)channel].source;
        }


        public void PlayBGMInIdleChannel(SoundChannelType type, string key, bool loop, float volume = 1.0f, float pitch = 1.0f)
        {
            SoundChannel channel = SoundChannel.UNKNOWN;
            int channelIdxStart = (int)type;
            int channelCnt = GetChannelCnt(type);

            for (int i = channelIdxStart, end = channelIdxStart + channelCnt; i < end; i++)
            {
                if (_channels[i].source.isPlaying == false)
                {
                    channel = (SoundChannel)i;
                }
            }

            Play(channel, key, loop, volume, pitch);
        }

        public void StopAt(SoundChannel channel)
        {
            var source = GetSource(channel);

            if(source == null)
            {
                return;
            }

            source.Stop();
        }

        public void StopAll()
        {
            foreach (var source in _channels)
            {
                if(source.source != null)
                {
                    source.source.Stop();
                }
            }
        }

        public void StopAllOf(SoundChannelType type)
        {
            int channelIdxStart = (int)type;
            int channelCnt = GetChannelCnt(type);

            for(int i=channelIdxStart, end=channelIdxStart+channelCnt; i < end;i++)
            {
                _channels[i].source.Stop();
            }
        }

        public void Clear()
        {
            StopAll();
            _cachedSounds.Clear();
            for(int i = 0; i < _pauseTimes.Length; i++)
            {
                _pauseTimes[i] = 0;
                _pauseStartedTimes[i] = 0;
            }
            AudioListener.pause = false;
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