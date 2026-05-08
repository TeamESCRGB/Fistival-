using Data;
using Defines;
using System;
using UnityEngine;

namespace Coordinator.Modes
{
    public class RootShooterMode : ModeBase
    {
        public override ModeTypes ModeType => ModeTypes.ROOT_SHOOTER;
        public override void Init(CommonModeData data)
        {
            base.Init(data);

        }

        public override void DeInit()
        {

            base.DeInit();
        }

        public override void OnDropEvent(bool pressed)
        {
            throw new NotImplementedException();
        }

        public override void OnLMBEvent(bool pressed, Vector2 screenPos)
        {
            throw new NotImplementedException();
        }

        public override void OnRMBEvent(bool pressed, Vector2 screenPos)
        {
            throw new NotImplementedException();
        }
        protected override void OnStunEnd()
        {
            _isStunned = false;
        }

        public override void StunFor(float time)
        {
            if (time <= 0)
            {
                return;
            }

            if (_stunCounter.IsCooldownEnded())
            {

            }

            _stunCounter.SetCooldownTime(time);
            _stunCounter.StartCooldown();
            _isStunned = true;
        }

        public override void ReleaseStun()
        {
            if (_stunCounter is null || _stunCounter.IsCooldownEnded())
            {
                return;
            }
            _stunCounter.StopCooldown();
        }
    }
}
