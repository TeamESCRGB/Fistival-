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

        private RhythmStatus _nextBeatType;

        private bool _isLoop = false;

        private bool _isPlaying = false;

        private void Update()
        {
            
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
            _noteIdx = 0;
            _notes = null;
            _onExactTime = null;
            _onLateTime = null;
            _nextBeatType = RhythmStatus.EXACT_BEAT;
        }
    }
}