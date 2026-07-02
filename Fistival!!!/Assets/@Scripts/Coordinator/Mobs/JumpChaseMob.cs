using Coordinator.MobActs;
using Coordinator.Movements;
using Coordinator.Skills;
using Coordinator.TriggerMovement;
using Data;
using Defines;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Coordinator.Mobs
{
    public class JumpChaseMob : MobCoordinatorBase
    {
        [SerializeField]
        protected int _damage;
        [SerializeField]
        protected float _stunTime;
        [SerializeField]
        protected float _knockbackForce;


        [SerializeField]
        protected float _jumpDelay;
        [SerializeField]
        protected float _jumpPow;
        protected PlatformerMovementCoordinator _move;
        protected JumpMovementAct _act;


        protected bool _isActing;
        protected bool _isAggroOn;

        protected float _skillTime;

        protected override void OnAwake()
        {
            base.OnAwake();
            _move = GetComponentInChildren<PlatformerMovementCoordinator>();
            _act = GetComponentInChildren<JumpMovementAct>();
        }

        public override void Init(CommonMobData data)
        {
            base.Init(data);
            _isActing = false;
            _isAggroOn = false;
            _skillTime = _skillDelay;
            GetComponentInChildren<TouchDamageSkill>().Init(data.PlayerHitboxLayer, _damage, _stunTime, _knockbackForce);
            _move.Init(data.Speed, _jumpPow, 1,1,GetComponent<Rigidbody2D>());
            _act.Init(() => { _isActing = false; Debug.Log("end!"); }, _animator, _move, GetComponent<Rigidbody2D>(), data.Speed, _jumpDelay);
        }

        protected override void OnAggroStateChanged(bool isAggroOn, Collider2D player)
        {
            _isAggroOn=isAggroOn;
        }

        private void Update()
        {
            if(_isActing)
            {
                return;
            }

            if(_skillTime < _skillDelay)
            {
                _skillTime += Time.deltaTime;
                return;
            }

            if(_stunCounter.IsCooldownEnded() == false || _isAggroOn == false)
            {
                return;
            }

            _skillTime = 0;
            _isActing = true;
            _act.Act();
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
    }
}
