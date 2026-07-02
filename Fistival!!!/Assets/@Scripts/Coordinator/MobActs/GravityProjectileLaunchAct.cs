using Coordinator;
using System;
using System.Collections;
using UnityEngine;
using Utils;

namespace Assets._Scripts.Coordinator.MobActs
{
    public class GravityProjectileLaunchAct : MobActBase
    {
        private int _projectileIdx;
        private WaitForSeconds _waiter;
        private int _shootCnt;
        private LayerMask _target;
        private Transform _player;
        private bool _isRunning = false;
        //애니메이터도 추가할것
        //소리도

        public void Init(Action onEnd, int projectileIdx,int shootCnt ,float shootTimeInterval, LayerMask attackTargetMask)
        {
            Init(onEnd);
            _isRunning = false;
            _target = attackTargetMask;
            _waiter = new WaitForSeconds(shootTimeInterval);
            _projectileIdx = projectileIdx;
            _shootCnt = shootCnt;
            _player = FindAnyObjectByType<PlayerCoordinator>().transform;
        }

        private IEnumerator ActRoutine()
        {
            _isRunning = true;
            var pos = transform.position;
            for (int i = 0; i < _shootCnt; i++)
            {
                ProjectileLaunchHelper.LaunchGravityProjectile(_target, _projectileIdx, pos, _player);
                yield return _waiter;
            }
            _isRunning = false;
            _onActEnd?.Invoke();
        }

        public override void Act()
        {
            _routine = StartCoroutine(ActRoutine());
        }

        public override void StopAct()
        {
            if(_routine != null && _isRunning)
            {
                StopCoroutine(_routine);
            }
            _onActEnd?.Invoke();
        }
    }
}
