using Coordinator.Hands;
using Coordinator.Movements;
using Data;
using Defines;
using UnityEngine;
using Utils;

namespace Coordinator.Modes
{
    public class RhythmMode : ModeBase
    {

        private RhythmHandCoordinator _hand;
        private PlatformerMovementCoordinator _movementCoordinator;
        private float _objectWeight = 0;

        public override ModeTypes ModeType => ModeTypes.RHYTHM;

        protected override void OnAwake()
        {
            base.OnAwake();
            _hand = gameObject.GetComponentInChildren<RhythmHandCoordinator>();
            _movementCoordinator = gameObject.GetOrAddComponent<PlatformerMovementCoordinator>();
        }
        protected override void OnFixedUpdate()
        {
            base.OnFixedUpdate();
            _animator.SetBool("IsWalking", _movementCoordinator.GetNowMoveDir() != Directions.OFF);
        }
        public override void Init(CommonModeData data)
        {
            base.Init(data);
            //초기화 로직
            _hand.Init(GetComponentInParent<Rigidbody2D>(), _playerData);
            _movementCoordinator.Init(_playerData.MoveSpeed, _playerData.JumpPower, _playerData.SlownessSensitivity, _playerData.MaxSlowness, GetComponentInParent<Rigidbody2D>());

            _objectWeight = 0;
            _inputCoordinator.SetJumpsMovementInputHandler(_movementCoordinator);
            _inputCoordinator.SetHorizontalMovementInputHandler(_movementCoordinator);

            _hand.OnGrabbedObjectChanged += OnGrabbedObjectChanged;
            _hand.OnChargeRateChanged += OnChargeRateChanged;
            _hand.SetMaxCharge(_playerData.MaxChargeCnt);
        }

        public override void DeInit()
        {
            //해제 로직
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
            if (objData == null)
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
            _movementCoordinator.SetSlowness(1 / (1 + (now / max * _objectWeight)));
        }

        public override void OnDropEvent(bool pressed)
        {
            if(pressed && (_isStunned == false))
            {
                _hand.Drop();
            }
        }

        public override void OnLMBEvent(bool pressed, Vector2 screenPos)
        {
            if(_isStunned)
            {
                return;
            }
            _hand.SetMousePos(screenPos);
            if(pressed)
            {
                _hand.OnLMBPressed();
            }
            else
            {
                _hand.OnLMBReleased();
            }
        }

        public override void OnRMBEvent(bool pressed, Vector2 screenPos)
        {
            if(_isStunned)
            {
                return;
            }
            if (pressed)
            {
                _hand.OnRMBPressed();
            }
            else
            {
                _hand.SetMousePos(screenPos);
                _hand.OnRMBReleased();
            }
        }

        protected override void OnStunEnd()
        {
            _isStunned = false;
            _movementCoordinator.UnlockMovement();
        }

        public override void StunFor(float time)
        {
            if (time <= 0 || _stunCounter.GetRemainedTime() >= time)
            {
                return;
            }

            if (_stunCounter.IsCooldownEnded())
            {
                _hand.StopAttack();
                _hand.Drop();
                _movementCoordinator.LockMovement();
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