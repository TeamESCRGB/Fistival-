
using Coordinator.Hands;
using Coordinator.Movements;
using Data;
using Defines;
using UnityEngine;
using Utils;

namespace Coordinator.Modes
{
    public class Shoot2DMode : ModeBase
    {
        [SerializeField]
        private int _projectileIdx;
        private FlightMovementCoordinator _movementCoord;
        public override ModeTypes ModeType => ModeTypes.SHOOT_2D;
        private Rigidbody2D _parentrb2d;
        protected ShooterHand _shooterHand;
        private float _gravityScale;

        protected override void OnAwake()
        {
            base.OnAwake();
            _parentrb2d = GetComponentInParent<Rigidbody2D>();
            _movementCoord = gameObject.GetOrAddComponent<FlightMovementCoordinator>();
            _shooterHand = GetComponentInChildren<ShooterHand>();
        }

        public override void Init(CommonModeData data)
        {
            base.Init(data);
            _movementCoord.Init(data.MoveSpeed, _parentrb2d);
            _gravityScale = _parentrb2d.gravityScale;
            _parentrb2d.gravityScale = 0;
            _inputCoordinator.SetHorizontalMovementInputHandler(_movementCoord);
            _inputCoordinator.SetVerticalMovementInputHandler(_movementCoord);

            _shooterHand.Init(_projectileIdx,GetComponentInParent<Rigidbody2D>() ,data.AttackableLayers, data.PickableLayers, data.AttackCooldown, data.ForcePerCharge, data.ChargeTimeInterval, data.AttackCooldown);
        }

        public override void DeInit()
        {
            _parentrb2d.gravityScale = _gravityScale;
            base.DeInit();
        }

        public override void OnDropEvent(bool pressed)
        {
            _shooterHand.Drop();
        }

        public override void OnLMBEvent(bool pressed, Vector2 screenPos)
        {
            if(pressed)
            {
                _shooterHand.OnLMBPressed();
            }
            else
            {
                _shooterHand.OnLMBReleased();
            }
        }

        public override void OnRMBEvent(bool pressed, Vector2 screenPos)
        {
            _shooterHand.SetMousePos(screenPos);
            if(pressed)
            {
                _shooterHand.OnRMBPressed();
            }
            else
            {
                _shooterHand.OnRMBReleased();
            }
        }
    }
}
