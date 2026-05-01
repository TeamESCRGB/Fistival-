

using Data;
using Defines;
using UnityEngine;

namespace Coordinator.Modes
{
    public class Shoot2DMode : ModeBase
    {
        public override ModeTypes ModeType => ModeTypes.SHOOT_2D;

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
            throw new System.NotImplementedException();
        }

        public override void OnLMBEvent(bool pressed, Vector2 screenPos)
        {
            throw new System.NotImplementedException();
        }

        public override void OnRMBEvent(bool pressed, Vector2 screenPos)
        {
            throw new System.NotImplementedException();
        }
    }
}
