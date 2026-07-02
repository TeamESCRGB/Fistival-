using Coordinator.MobActs;
using Coordinator.Movements;
using Coordinator.Skills;
using Data;
using UnityEngine;

namespace Coordinator.Mobs
{
    public class TurtleMob : MobCoordinatorBase
    {
        [SerializeField]
        protected int _damage;
        [SerializeField]
        protected float _stunTime;
        [SerializeField]
        protected float _knockbackForce;

        protected bool _isActing;
        protected bool _isAggroOn;
        protected float _skillTime;

        protected DashAct _act;
        protected PlatformerMovementCoordinator _mov;

        protected override void OnAwake()
        {
            base.OnAwake();
            _mov = GetComponent<PlatformerMovementCoordinator>();
            _act = GetComponentInChildren<DashAct>();
        }

        public override void Init(CommonMobData data)
        {
            base.Init(data);
            _isActing = false;
            _isAggroOn = false;
            _skillTime = _skillDelay;
            GetComponentInChildren<TouchDamageSkill>().Init(data.PlayerHitboxLayer, _damage, _stunTime, _knockbackForce);
            _mov.Init(data.Speed,0,1,1,GetComponent<Rigidbody2D>());
            _act.Init(() => { _isActing = false; }, _animator, _mov, GetComponent<Rigidbody2D>(), data.Speed);
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

            if (_stunCounter.IsCooldownEnded() == false || _isAggroOn == false)
            {
                return;
            }

            _skillTime = 0;
            _isActing = true;
            _act.Act();
        }

        protected override void OnAggroStateChanged(bool isAggro, Collider2D collider)
        {
            _isAggroOn= isAggro;
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
                _act.StopAct();
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
