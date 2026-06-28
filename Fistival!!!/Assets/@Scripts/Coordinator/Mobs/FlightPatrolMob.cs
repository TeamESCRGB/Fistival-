using Coordinator.Movements.TriggerMovement;
using Coordinator.Skills;
using Data;
using Defines;
using UnityEngine;

namespace Coordinator.Mobs
{
    public class FlightPatrolMob : MobCoordinatorBase
    {
        [SerializeField]
        protected int _damage;
        [SerializeField]
        protected float _stunTime;
        [SerializeField]
        protected float _knockbackForce;
        [SerializeField]
        protected MovementKeyStatus _initialDir;
        protected FlightTriggerMovementCoordinator _move;
        

        protected override void OnAwake()
        {
            base.OnAwake();
            _move = GetComponentInChildren<FlightTriggerMovementCoordinator>();
        }

        public override void Init(CommonMobData data)
        {
            base.Init(data);
            GetComponentInChildren<TouchDamageSkill>().Init(data.PlayerHitboxLayer, _damage, _stunTime, _knockbackForce);
            _move.Init(data.Speed, 0, GetComponent<Rigidbody2D>(), _initialDir);
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

        #region Unused
        protected override void OnAggroStateChanged(bool isAggro, Collider2D collider)
        {

        }
        #endregion

    }
}