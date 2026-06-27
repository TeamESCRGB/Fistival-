using Data;
using UnityEngine;
using Utils;
using Coordinator.Hands;
using Coordinator.Movements;
using Defines;

namespace Coordinator.Modes
{
    public class FistivalMode : ModeBase
    {
        protected HandCoordinator _hand;
        private PlatformerMovementCoordinator _movCoordinator;
        private float _objectWeight = 0;
        public override ModeTypes ModeType => ModeTypes.FISTIVAL;


        protected override void OnAwake()
        {
            base.OnAwake();
            _movCoordinator = gameObject.GetOrAddComponent<PlatformerMovementCoordinator>();
            _hand = GetComponentInChildren<HandCoordinator>();
        }

        public override void Init(CommonModeData data)
        {
            base.Init(data);
            _movCoordinator.Init(_playerData.MoveSpeed, _playerData.JumpPower, _playerData.SlownessSensitivity, _playerData.MaxSlowness,GetComponentInParent<Rigidbody2D>());
            _inputCoordinator.SetJumpsMovementInputHandler(_movCoordinator);
            _inputCoordinator.SetHorizontalMovementInputHandler(_movCoordinator);
            _hand.Init(GetComponentInParent<Rigidbody2D>(), _playerData);
            _hand.OnGrabbedObjectChanged+=OnGrabbedObjectChanged;
            _hand.OnChargeRateChanged += OnChargeRateChanged;
            _hand.SetMaxCharge(_playerData.MaxChargeCnt);
            _objectWeight = 0;
        }

        public override void DeInit()
        {
            _hand.Drop();
            _hand.StopAttack();
            base.DeInit();
        }

        public override void UpdateUpdatedPlayerData()
        {
            _hand.UpdateUpdatedData(_playerData);
        }

        protected override void OnInputActionMapChanged(ActionMapTypes mapType)
        {
            base.OnInputActionMapChanged(mapType);

            if (mapType != ActionMapTypes.PLAYER)
            {
                _hand.StopAttack();
                _hand.StopCharging();
            }
        }

        private void OnGrabbedObjectChanged(ObjectData objData)
        {
            if(objData == null)
            {
                _objectWeight = 0;
            }
            else
            {
                _objectWeight = objData.Weight;
            }

        }

        private void OnChargeRateChanged(int now, int max)
        {
            _movCoordinator.SetSlowness(1/(1 + (now / max * _objectWeight)));
        }

        public override void OnRMBEvent(bool pressed, Vector2 screenPos)
        {
            if(_isStunned)
            {
                return;
            }
            if(pressed)
            {
                _hand.OnRMBPressed();
            }
            else
            {
                _hand.SetMousePos(screenPos);
                _hand.OnRMBReleased();
            }
        }

        public override void OnLMBEvent(bool pressed, Vector2 screenPos)
        {
            if(_isStunned)
            {
                return;
            }
            if(pressed)
            {
                _hand.OnLMBPressed();
            }
            else
            {
                _hand.OnLMBReleased();
            }
        }

        public override void OnDropEvent(bool pressed)
        {
            if(pressed && (_isStunned == false))
            {
                _hand.Drop();
            }
        }
        protected override void OnStunEnd()
        {
            _isStunned = false;
            _movCoordinator.UnlockMovement();
        }

        public override void StunFor(float time)
        {
            if (time <= 0 || _stunCounter.GetRemainedTime() >= time)
            {
                return;
            }

            if (_stunCounter.IsCooldownEnded())
            {
                _movCoordinator.LockMovement();
                _hand.Drop();
                _hand.StopAttack();
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
                _hand.Interact();
            }
        }
    }
}