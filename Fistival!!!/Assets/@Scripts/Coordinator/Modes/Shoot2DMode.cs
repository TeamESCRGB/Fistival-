

using Assets._Scripts.Coordinator.Movements;
using Data;
using Defines;
using UnityEngine;
using Utils;

namespace Coordinator.Modes
{
    public class Shoot2DMode : ModeBase
    {
        private FlightMovementCoordinator _movementCoord;
        public override ModeTypes ModeType => ModeTypes.SHOOT_2D;
        private Rigidbody2D _parentrb2d;
        private float _gravityScale;

        protected override void OnAwake()
        {
            base.OnAwake();
            _parentrb2d = GetComponentInParent<Rigidbody2D>();
            _movementCoord = gameObject.GetOrAddComponent<FlightMovementCoordinator>();
        }

        public override void Init(CommonModeData data)
        {
            base.Init(data);
            _movementCoord.Init(data.MoveSpeed, _parentrb2d);
            _gravityScale = _parentrb2d.gravityScale;
            _parentrb2d.gravityScale = 0;
            _inputCoordinator.SetHorizontalMovementInputHandler(_movementCoord);
            _inputCoordinator.SetVerticalMovementInputHandler(_movementCoord);
        }

        public override void DeInit()
        {
            _parentrb2d.gravityScale = _gravityScale;
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
