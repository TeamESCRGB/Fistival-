using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Utils;

namespace Coordinator.MobActs.PlatformerBoss
{
    public class PlatformerPhase2Fist : MobActBase
    {
        private BlockWaveCoordinator _wave;

        [SerializeField]
        private float _speed;
        [SerializeField]
        private float _amplitude;
        [SerializeField]
        private float _width;

        [SerializeField]
        private int _spawnObjectIdx;

        private IReadOnlyList<Transform> _points;

        private bool _isObjectSpawned;

        public void Init(Action onActionEnd, Animator animator, BlockWaveCoordinator wave, IReadOnlyList<Transform> points)
        {
            Init(onActionEnd, animator);
            _wave = wave;
            _points=points;
            _isObjectSpawned = false;
        }

        public void EndPlatformerPhase2Fist()
        {
            _onActEnd?.Invoke();
        }

        private void OnPlatformerPhase2FistWaveProceed(float nowCenter, float boxCount)
        {
            if(_isObjectSpawned)
            {
                return;
            }

            if(nowCenter > boxCount - boxCount/4)
            {
                ObjectSpawnHelper.Spawn(_spawnObjectIdx, _points[UnityEngine.Random.Range(0,_points.Count)]);
                _isObjectSpawned = true;
            }
        }

        private void OnPlatformerPhase2FistWaveEnd(bool result)
        {
            _animator.SetTrigger("PlatformerPhase2RetrievePunch");
            _wave.OnEnd -= OnPlatformerPhase2FistWaveEnd;
        }

        public void StartPlatformerPhase2FistWave()
        {
            _wave.OnProceed -= OnPlatformerPhase2FistWaveProceed;
            _wave.OnProceed += OnPlatformerPhase2FistWaveProceed;
            _wave.OnEnd -= OnPlatformerPhase2FistWaveEnd;
            _wave.OnEnd += OnPlatformerPhase2FistWaveEnd;
            _wave.TriggerWave(_speed, _amplitude, _width);
        }

        public void OnPlatformerPhase2FistPunchComplete()
        {
            //여기에 이제 소리나 이펙트 넣기
        }

        public override void Act()
        {
            _isObjectSpawned = false;
            _animator.SetTrigger("PlatformerPhase2Punch");
        }

        private void OnDisable()
        {
            if(_wave != null)
            {
                _wave.OnProceed -= OnPlatformerPhase2FistWaveProceed;
                _wave.OnEnd -= OnPlatformerPhase2FistWaveEnd;
            }
        }

        public override void StopAct()
        {

        }

        //펀치 착지->충격파 이동->충격파 이동 끝->펀치 회수
    }
}