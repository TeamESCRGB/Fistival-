using Coordinator.Rhythm;
using Data;
using Defines;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Manager.Contents
{
    public class RhythmModeManager : MonoBehaviour
    {
        private event Action<int,NoteTypes> _onExactTime;
        private event Action<int,NoteTypes> _onLateTime;

        private IReadOnlyList<NoteData> _notes = null;

        private int _noteIdx = 0;

        private double _perfectRange = 0;
        private double _goodRange = 0;

        private string _musicKey = "";

        private RhythmStatus _nextBeatType;

        private bool _isLoop = false;

        private bool _isPlaying = false;

        private double _oldTime;

        private bool _isPaused = false;

        private const JudgementTypes _missMask = JudgementTypes.EARLY_MISS | JudgementTypes.LATE_MISS;


        private void Update()
        {
            if(_isPaused)
            {
                return;
            }
            if(_isPlaying == false)
            {
                if(_isLoop == false)
                {
                    return;
                    
                }
                InitPlayStatus();
                Managers.Instance.GlobalSoundManager.Play(SoundChannel.BGM_0, _musicKey, false);
                _isPlaying = Managers.Instance.GlobalSoundManager.IsPlaying(SoundChannel.BGM_0);
            }


            double dspTime = Managers.Instance.GlobalSoundManager.GetDSPTime(SoundChannel.BGM_0);

            if(dspTime <= 0 && _oldTime > 0)
            {
                _isPlaying = false;Debug.Log("asdf");
                return;
            }

            _oldTime = dspTime;

            if (_noteIdx >= _notes.Count)
            {
                return;
            }
            
            NoteTypes noteType = _notes[_noteIdx].NoteType;
            switch(CheckJudgementType(dspTime, _notes[_noteIdx].Timing))
            {
                case JudgementTypes.PERFECT:
                    if(_nextBeatType == RhythmStatus.EXACT_BEAT)
                    {
                        _nextBeatType = RhythmStatus.LATE_BEAT;
                        _onExactTime?.Invoke(_noteIdx,noteType);
                    }
                    break;
                case JudgementTypes.LATE_MISS:
                    if (_nextBeatType == RhythmStatus.LATE_BEAT)
                    {
                        _nextBeatType = RhythmStatus.EXACT_BEAT;
                        _onLateTime?.Invoke(_noteIdx, noteType);
                        _noteIdx++;
                    }
                    break;
            }

        }

        /*
         그냥 빈 노트도 전부 패턴파일에 포함되도록 한다 <- 이게 로직 짜기 편할거같음. <- 지금은 따로 더 해서 한다? 욕심임 일정 밀렸어
         */

        public void PausePattern()
        {
            _isPaused = true;
            Managers.Instance.GlobalSoundManager.Pause(SoundChannel.BGM_0);
        }

        public void UnPausePattern()
        {
            Managers.Instance.GlobalSoundManager.UnPause(SoundChannel.BGM_0);
            _isPaused = false;
        }

        private JudgementTypes CheckJudgementType(double time, double noteTime)
        {
            if (_noteIdx >= _notes.Count)
            {
                return 0;
            }


            if ((noteTime - _perfectRange <= time) && (noteTime + _perfectRange >= time))
            {
                return JudgementTypes.PERFECT;
            }
            else if ((noteTime - _goodRange <= time) && (noteTime + _goodRange >= time))
            {
                return JudgementTypes.GOOD;
            }
            else if (noteTime - _goodRange > time)
            {
                return JudgementTypes.EARLY_MISS;
            }
            else
            {
                return JudgementTypes.LATE_MISS;
                
            }

        }

        private void InitPlayStatus()
        {
            _isPaused = false;
            _oldTime = 0;
            _noteIdx = 0;
            _nextBeatType = RhythmStatus.EXACT_BEAT;
            Managers.Instance.GlobalSoundManager.StopAt(SoundChannel.BGM_0);
        }

        public void RegisterOnExactTime(IRhythmReceiver receiver)
        {
            if(receiver is null)
            {
                return;
            }

            _onExactTime -= receiver.OnExactBPM;
            _onExactTime += receiver.OnExactBPM;
        }



        public (int nowIdx, int endIdx, NoteTypes noteType, JudgementTypes judgeType) ClickParry()
        {
            if(_noteIdx >= _notes.Count)
            {
                return (-1, -1, 0, 0);
            }

            NoteTypes noteType = _notes[_noteIdx].NoteType;
            JudgementTypes judgement = CheckJudgementType(Managers.Instance.GlobalSoundManager.GetDSPTime(SoundChannel.BGM_0), _notes[_noteIdx].Timing);

            if((judgement & _missMask) != 0)
            {
                return (-1, -1, noteType, judgement);
            }
            
            if(noteType == NoteTypes.LONG_PARRY_START)
            {
                return (_noteIdx, _noteIdx + 2, NoteTypes.LONG_PARRY_START, judgement);
            }

            return (_noteIdx, _noteIdx, noteType, judgement);
        }

        public void RegisterOnLateTime(IRhythmReceiver receiver)
        {
            if (receiver is null)
            {
                return;
            }

            _onLateTime -= receiver.OnLateBPM;
            _onLateTime += receiver.OnLateBPM;
        }

        public void UnregisterOnExactTime(IRhythmReceiver receiver)
        {
            if (receiver is null)
            {
                return;
            }

            _onExactTime -= receiver.OnExactBPM;
        }

        public void UnregisterOnLateTime(IRhythmReceiver receiver)
        {
            if (receiver is null)
            {
                return;
            }

            _onLateTime -= receiver.OnLateBPM;
        }

        public void Clear()
        {
            _isPlaying = false;
            _isLoop = false;
            _perfectRange = 0;
            _goodRange = 0;
            _notes = null;
            _onExactTime = null;
            _onLateTime = null;
            _musicKey = "";
            InitPlayStatus();
        }

        public void StartPattern(string patternName)
        {
            var pattern = Managers.Instance.DataManager.PatternDataDict[patternName];

            if(pattern is null)
            {
                return;
            }

            InitPlayStatus();
            _goodRange = pattern.GoodRange;
            _perfectRange = pattern.PerfactRange;
            _musicKey = pattern.SongName;
            _notes = pattern.Notes;
            _isLoop = pattern.IsLooping;

            Managers.Instance.GlobalSoundManager.Play(SoundChannel.BGM_0,_musicKey,false);
            _isPlaying = Managers.Instance.GlobalSoundManager.IsPlaying(SoundChannel.BGM_0);
        }
    }
}