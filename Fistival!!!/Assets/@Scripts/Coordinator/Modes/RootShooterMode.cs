using Coordinator.Hands;
using Coordinator.Movements;
using Data;
using Defines;
using InputHandler;
using UnityEngine;
using Utils;

namespace Coordinator.Modes
{
    public class RootShooterMode : ModeBase, IReloadInputHandler
    {
        private PlatformerMovementCoordinator _movCoordinator;
        private RootShooterHand _hand;
        private float _objectWeight = 0;

        public override ModeTypes ModeType => ModeTypes.ROOT_SHOOTER;

        protected override void OnAwake()
        {
            base.OnAwake();
            _movCoordinator = gameObject.GetOrAddComponent<PlatformerMovementCoordinator>();
            _hand = GetComponentInChildren<RootShooterHand>();
        }
        protected override void OnFixedUpdate()
        {
            base.OnFixedUpdate();
            _animator.SetBool("IsWalking", _movCoordinator.GetNowMoveDir() != Directions.OFF);
        }
        public override void Init(CommonModeData data)
        {
            base.Init(data);
            _movCoordinator.Init(_playerData.MoveSpeed, _playerData.JumpPower, _playerData.SlownessSensitivity, _playerData.MaxSlowness, GetComponentInParent<Rigidbody2D>(), _movementSoundKeys);
            _inputCoordinator.SetJumpsMovementInputHandler(_movCoordinator);
            _inputCoordinator.SetHorizontalMovementInputHandler(_movCoordinator);
            _inputCoordinator.SetPointerMovementInputHandler(_hand);
            _inputCoordinator.SetReloadInputHandler(this);
            _hand.Init(GetComponentInParent<Rigidbody2D>(), _playerData);//_playerData.Damage, _playerData.AttackableLayers, _playerData.PickableLayers, _playerData.ForcePerCharge, _playerData.ChargeTimeInterval, _playerData.AttackCooldown, _playerData.ThrowAttackAdditionalDamage, _playerData.StrongAttackThreshold, _playerData.StunTime
            _hand.OnChargeRateChanged += OnChargeRateChanged;
            _hand.OnGrabbedObjectChanged += OnGrabbedObjectChanged;
            _hand.SetMaxCharge(_playerData.MaxChargeCnt);
            _objectWeight = 0;
        }

        public override void DeInit()
        {
            _hand.Drop();
            _hand.StopAttack();
            _hand.DeInit();
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
            _movCoordinator.SetSlowness(1 / (1 + (now / max * _objectWeight)));
        }

        public override void OnDropEvent(bool pressed)
        {
            if (pressed && (_isStunned == false))
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
            if (_isStunned)
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
        public void OnReloadInputEvent(bool pressed)
        {
            if(pressed && (_isStunned == false))
            {
                _hand.Reload();
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