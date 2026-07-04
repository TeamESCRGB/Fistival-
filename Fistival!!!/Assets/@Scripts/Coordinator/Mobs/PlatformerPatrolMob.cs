using ComponentModule;
using Coordinator.Movements;
using Coordinator.Skills;
using Coordinator.TriggerMovement;
using Data;
using Defines;
using Manager;
using UnityEngine;

namespace Coordinator.Mobs
{
    public class PlatformerPatrolMob : MobCoordinatorBase
    {
        protected PlatfoermerTriggerMovementCoordinator _move;
        

        protected override void OnAwake()
        {
            base.OnAwake();
            _move = GetComponentInChildren<PlatfoermerTriggerMovementCoordinator>();
        }

        public override void Init(CommonMobData data)
        {
            base.Init(data);
            GetComponentInChildren<TouchDamageSkill>().Init(data.PlayerHitboxLayer, _damage, _stunTime, _knockbackForce);
            _move.Init(data.Speed, 0, GetComponent<Rigidbody2D>(), (MovementKeyStatus)data.InitialDir);
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