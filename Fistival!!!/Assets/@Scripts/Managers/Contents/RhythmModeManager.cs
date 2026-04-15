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
        private event Action<bool,IParrableObject,NoteTypes> _onParry;

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

        private const NoteTypes _noActionMask = NoteTypes.SHORT_PARRY_RDY | NoteTypes.LONG_PARRY_RDY | NoteTypes.LONG_PARRY_MIDDLE | NoteTypes.LONG_PARRY_START | NoteTypes.NO_ACTION;

        private bool _isParried = false;


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
                        if((noteType & _noActionMask) == 0 && _isParried == false)
                        {
                            _onParry?.Invoke(false, null, noteType);
                        }
                        _nextBeatType = RhythmStatus.EXACT_BEAT;
                        _noteIdx++;
                        _isParried = false;
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
            _isParried = false;
            _isPaused = false;
            _oldTime = 0;
            _noteIdx = 0;
            _nextBeatType = RhythmStatus.EXACT_BEAT;
            Managers.Instance.GlobalSoundManager.StopAt(SoundChannel.BGM_0);
        }

        public void RegisterOnExactTime(IExactRhythmReceiver receiver)
        {
            if(receiver is null)
            {
                return;
            }

            _onExactTime -= receiver.OnExactBPM;
            _onExactTime += receiver.OnExactBPM;
        }



        public (int nowIdx, int endIdx, NoteTypes noteType, JudgementTypes judgeType) ClickParry(IParrableObject parrableObject)
        {
            if(_noteIdx >= _notes.Count || _isPlaying == false || _isPaused)
            {
                return (-1, -1, 0, _missMask);
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
            
            if((noteType & _noActionMask) == 0 && _isParried == false)
            {
                _onParry?.Invoke(true, parrableObject, noteType);
                _isParried = true;
            }

            if(noteType == NoteTypes.LONG_PARRY_END)
            {
                return (_noteIdx, -1, NoteTypes.LONG_PARRY_END, judgement);
            }

            return (_noteIdx, _noteIdx, noteType, judgement);
        }


        public (int nowIdx, NoteTypes noteType, JudgementTypes judgeType) ReleaseParry(int end, IParrableObject parrableObject)
        {
            if (_noteIdx >= _notes.Count || _isPlaying == false || _isPaused)
            {
                return (-1, 0, _missMask);
            }

            NoteTypes noteType = _notes[_noteIdx].NoteType;

            if (noteType != NoteTypes.LONG_PARRY_END || _noteIdx != end)
            {
                return (_noteIdx, noteType, _missMask);
            }

            JudgementTypes judgement = CheckJudgementType(Managers.Instance.GlobalSoundManager.GetDSPTime(SoundChannel.BGM_0), _notes[_noteIdx].Timing);

            if((judgement & _missMask) == 0)
            {
                _onParry?.Invoke(true, parrableObject, NoteTypes.LONG_PARRY_END);
                _isParried = true;
            }

            return (_noteIdx, NoteTypes.LONG_PARRY_END, judgement);
        }

        public void RegisterOnParry(IParryResultReceiver receiver)
        {
            if (receiver is null)
            {
                return;
            }

            _onParry -= receiver.OnParry;
            _onParry += receiver.OnParry;
        }

        public void UnregisterOnExactTime(IExactRhythmReceiver receiver)
        {
            if (receiver is null)
            {
                return;
            }

            _onExactTime -= receiver.OnExactBPM;
        }

        public void UnregisterOnParry(IParryResultReceiver receiver)
        {
            if (receiver is null)
            {
                return;
            }

            _onParry -= receiver.OnParry;
        }

        public void Clear()
        {
            _isPlaying = false;
            _isLoop = false;
            _perfectRange = 0;
            _goodRange = 0;
            _notes = null;
            _onExactTime = null;
            _onParry = null;
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