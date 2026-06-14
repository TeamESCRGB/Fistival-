
using Coordinator.Hands;
using Coordinator.Movements;
using Data;
using Defines;
using UnityEngine;
using UnityEngine.XR;
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
        private bool _isLMBPressed;

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
            _isLMBPressed = false;
            _movementCoord.Init(_playerData.MoveSpeed, _parentrb2d);
            _gravityScale = _parentrb2d.gravityScale;
            _parentrb2d.gravityScale = 0;
            _inputCoordinator.SetHorizontalMovementInputHandler(_movementCoord);
            _inputCoordinator.SetVerticalMovementInputHandler(_movementCoord);

            _shooterHand.Init(_projectileIdx,GetComponentInParent<Rigidbody2D>() ,_playerData.AttackableLayers, _playerData.PickableLayers, _playerData.AttackCooldown, _playerData.ForcePerCharge, _playerData.ChargeTimeInterval, _playerData.AttackCooldown);
        }

        public override void DeInit()
        {
            _parentrb2d.gravityScale = _gravityScale;
            _shooterHand.Drop();
            base.DeInit();
        }

        protected override void OnInputActionMapChanged(ActionMapTypes mapType)
        {
            base.OnInputActionMapChanged(mapType);

            if (mapType != ActionMapTypes.PLAYER)
            {
                _shooterHand.OnLMBReleased();
                _shooterHand.StopCharging();
            }
        }

        public override void OnDropEvent(bool pressed)
        {
            if(pressed && (_isStunned == false))
            {
                _shooterHand.Drop();
            }
        }

        public override void OnLMBEvent(bool pressed, Vector2 screenPos)
        {
            _isLMBPressed = pressed;
            if(_isStunned)
            {
                return;
            }
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
            if(_isStunned)
            {
                return;
            }
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

        protected override void OnStunEnd()
        {
            _isStunned = false;
            _movementCoord.UnlockMovement();
            if(_isLMBPressed)
            {
                _shooterHand.OnLMBPressed();
            }
        }

        public override void StunFor(float time)
        {
            if (time <= 0 || _stunCounter.GetRemainedTime() >= time)
            {
                return;
            }

            if (_stunCounter.IsCooldownEnded())
            {
                _movementCoord.LockMovement();
                _shooterHand.Drop();
                _shooterHand.OnLMBReleased();
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

        public override void OnInteractionInputEvent(bool pressed)
        {
            if (pressed && _isStunned == false)
            {
                _shooterHand.Interact();
            }
        }
    }
}
