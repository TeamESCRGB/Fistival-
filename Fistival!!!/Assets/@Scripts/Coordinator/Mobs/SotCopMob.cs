using Coordinator.MobActs;
using Coordinator.Movements;
using Coordinator.Skills;
using Data;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Coordinator.Mobs
{
    public class SotCopMob : MobCoordinatorBase
    {
        [SerializeField]
        protected int _damage;
        [SerializeField]
        protected float _stunTime;
        [SerializeField]
        protected float _knockbackForce;
        [SerializeField]
        protected float _jumpPow;

        [SerializeField]
        private int _slashProjectileIdx;
        [SerializeField]
        protected int _slashCnt;

        protected PlatformerMovementCoordinator _move;

        private MobActBase[] _acts = new MobActBase[3];

        [SerializeField]
        private float _jumpForce;

        protected Transform _player;
        protected bool _isActing;
        protected float _skillTime;

        [SerializeField]
        private string _slamDropObjPrefab;
        [SerializeField]
        private int _slamDropObjIdx;
        [SerializeField]
        private int _slamDropObjCnt;
        [SerializeField]
        private float _slamDropObjForce;
        [SerializeField]
        private LayerMask _attackLayer;

        [SerializeField]
        private float _dashStopTime;

        protected override void OnAwake()
        {
            base.OnAwake();
            _move = GetComponentInChildren<PlatformerMovementCoordinator>();
            _acts[0] = GetComponent<ProjectileLaunchAct>();
            _acts[1] = GetComponent<SlamAct>();
            _acts[2] = GetComponent<LengthDashAct>();
        }

        public override void Init(CommonMobData data)
        {
            base.Init(data);
            _isActing = false;
            _skillTime = _skillDelay;
            _player = FindAnyObjectByType<PlayerCoordinator>().transform;
            GetComponentInChildren<TouchDamageSkill>().Init(data.PlayerHitboxLayer, _damage, _stunTime, _knockbackForce);
            _move.Init(data.Speed, _jumpPow, 1, 1, GetComponent<Rigidbody2D>());
            ((ProjectileLaunchAct)_acts[0]).Init(() => { _isActing = false; }, _animator, _slashProjectileIdx, _slashCnt, data.PlayerHitboxLayer);
            ((SlamAct)_acts[1]).Init(() => { _isActing = false; }, _animator, GetComponent<Rigidbody2D>(),_slamDropObjPrefab,_slamDropObjIdx,_slamDropObjCnt ,_player, _jumpForce, _slamDropObjForce,_groundLayer,_attackLayer);
            ((LengthDashAct)_acts[2]).Init(() => { _isActing = false; }, _animator, _move, GetComponent<Rigidbody2D>(), data.Speed, _dashStopTime);
        }

        private void Update()
        {
            if (_isActing)
            {
                return;
            }

            if (_skillTime < _skillDelay)
            {
                _skillTime += Time.deltaTime;
                return;
            }

            if (_stunCounter.IsCooldownEnded() == false)
            {
                return;
            }

            _skillTime = 0;
            _isActing = true;

            if(_player.position.x < transform.position.x)
            {
                _move.OnLeftMovementInputEvent(true);
                _move.OnLeftMovementInputEvent(false);
            }
            else
            {
                _move.OnRightMovementInputEvent(true);
                _move.OnRightMovementInputEvent(false);
            }

            _acts[2].Act();//UnityEngine.Random.Range(0, _acts.Length)

        }



        public override void StunFor(float time)
        {
            if (time <= 0 || _stunCounter.GetRemainedTime() >= time)
            {
                return;
            }

            if (_stunCounter.IsCooldownEnded())
            {
                _movLock.LockMovement();
            }

            _stunCounter.SetCooldownTime(time);
            _stunCounter.StartCooldown();
        }

        public override void ReleaseStun()
        {
            if (_stunCounter is null || _stunCounter.IsCooldownEnded())
            {
                return;
            }
            _stunCounter.StopCooldown();
        }

        public override void OnStunEnd()
        {
            _movLock.UnlockMovement();
        }

        protected override void OnAggroStateChanged(bool isAggroOn, Collider2D player)
        {
            
        }
    }
}
