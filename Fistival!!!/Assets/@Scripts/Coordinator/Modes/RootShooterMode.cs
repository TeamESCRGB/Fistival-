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
    }
}
