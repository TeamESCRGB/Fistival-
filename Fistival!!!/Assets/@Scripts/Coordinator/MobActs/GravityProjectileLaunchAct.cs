using Coordinator;
using System;
using System.Collections;
using UnityEngine;
using Utils;

namespace Assets._Scripts.Coordinator.MobActs
{
    public class GravityProjectileLaunchAct : MobActBase
    {
        protected int _projectileIdx;
        protected int _maxShootCnt;
        protected int _shootCnt = 0;
        protected LayerMask _target;
        protected Transform _player;
        protected bool _isStopped = false;

        public void Init(Action onEnd,Animator animator ,int projectileIdx,int shootCnt, LayerMask attackTargetMask)
        {
            Init(onEnd,animator);
            _isStopped = false;
            _target = attackTargetMask;
            _projectileIdx = projectileIdx;
            _maxShootCnt = shootCnt;
            _player = FindAnyObjectByType<PlayerCoordinator>().transform;
        }

        public virtual void Launch()
        {
            if (_shootCnt <= 0)
            {
                return;
            }
            _shootCnt--;
            ProjectileLaunchHelper.LaunchGravityProjectile(_target,_projectileIdx,transform.position, _player);
            _animator.SetInteger("ShootCnt", _shootCnt);
        }


        public void End()
        {
            if(_isStopped == false)
            {
                _onActEnd?.Invoke();
            }
        }


        public override void Act()
        {
            _isStopped = false;
            _shootCnt = _maxShootCnt;
            _animator.SetInteger("ShootCnt", _shootCnt);
            _animator.SetTrigger("Shoot");
        }

        public override void StopAct()
        {
            _isStopped = true;
            _animator.SetInteger("ShootCnt", 0);
            _shootCnt = 0;
            _onActEnd?.Invoke();
        }
    }
}