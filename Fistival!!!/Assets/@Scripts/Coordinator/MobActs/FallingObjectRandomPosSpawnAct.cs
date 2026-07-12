using Assets._Scripts.Utils;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Coordinator.MobActs
{
    public class FallingObjectRandomPosSpawnAct : MobActBase
    {
        private IReadOnlyList<int> _fallingObjIdx;
        private Vector3 _posMin;
        private Vector3 _posMax;
        private float _interval;
        private float _timer;
        private int _idx;
        private bool _isActing;

        public void Init(Action onActionEnd, Animator animator, IReadOnlyList<int> fallingObjIdx, Vector3 posMin, Vector3 posMax, float interval)
        {
            Init(onActionEnd, animator);
            _isActing = false;
            _fallingObjIdx = fallingObjIdx;
            _interval = interval;
            _posMin = posMin;
            _posMax = posMax;
        }

        private void Update()
        {
            if(_isActing==false || _idx >= _fallingObjIdx.Count)
            {
                return;
            }

            _timer += Time.deltaTime;

            if(_timer >= _interval)
            {
                FallingObjectSpawnHelper.SpawnFallingObject(_fallingObjIdx[_idx],
                    new Vector3(
                        UnityEngine.Random.Range(_posMin.x, _posMax.x),
                        UnityEngine.Random.Range(_posMin.y, _posMax.y),
                        _posMax.z
                    ));
                _timer = 0;
                _idx++;
                if(_idx >= _fallingObjIdx.Count)
                {
                    _animator.SetTrigger("FallingObjectSpawnStop");
                }
            }
        }

        public void EndFallingObjectRandomSpawn()
        {
            _isActing = false;
            _onActEnd?.Invoke();
        }

        public void StartFallingObjectRandomSpawn()
        {
            _idx = 0;
        }

        public override void Act()
        {
            _timer = 0;
            _idx = _fallingObjIdx.Count+1;
            _isActing = true;
            _animator.SetTrigger("FallingObjectSpawn");
        }

        public override void StopAct()
        {
            if(_isActing == false)
            {
                return;
            }

            _isActing = false;
            _idx = 0;
            _timer = 0;
            _onActEnd?.Invoke();
        }
    }
}
