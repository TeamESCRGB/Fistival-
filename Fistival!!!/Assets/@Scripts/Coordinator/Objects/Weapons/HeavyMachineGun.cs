using ComponentModule;
using Coordinator.Objects.Weapons;
using Coordinator.Victims;
using Data;
using Manager;
using UnityEngine;
using Utils;

namespace Objects.Weapons
{
    public class HeavyMachineGun : WeaponCoordinatorBase
    {
        [SerializeField]
        private int _bullectProjectileIDX;
        [SerializeField]
        private int _explosionProjectileIDX;
        [SerializeField]
        private float _cooldownTime=0.2f;
        private bool _isPressed=false;

        private CooldownComponentModule _cooldownModule;

        protected override void OnDisabled()
        {
            if (Managers.Instance != null && _cooldownModule is not null)
            {
                Managers.Instance.CooldownManager.ReturnModule(_cooldownModule);
            }
            _cooldownModule = null;
            base.OnDisabled();
        }

        public override void Init(ObjectData data)
        {
            base.Init(data);
            _isPressed = false;

            _cooldownModule = Managers.Instance.CooldownManager.GetCooldownModule(_cooldownTime, -1);
        }

        protected override void InternalCollisionHandler(Collision2D col)
        {
            if (_isThrown == false)
            {
                return;
            }

            if (col == null || col.gameObject == null)
            {
                return;
            }

            if (col.gameObject.TryGetComponent<IAttackable>(out var comp) == false)
            {
                return;
            }

            if (((1 << col.gameObject.layer) & _abrasableLayerMask) != 0)
            {
                _durability--;
                ProjectileLaunchHelper.LaunchConstantDir(_attackableLayers, _explosionProjectileIDX, transform.position, transform.right);
            }
            else if (((1 << col.gameObject.layer) & _attackableLayers) != 0)
            {
                Managers.Instance.AttackManager.RequestAttack(comp, this, (int)(_baseDamage * _rb2d.linearVelocity.magnitude), _rb2d.linearVelocity, _chargeRate*_baseStunTime);
                _durability--;
                ProjectileLaunchHelper.LaunchConstantDir(_attackableLayers, _explosionProjectileIDX, transform.position, transform.right);
            }

            if (_durability <= 0)
            {
                Managers.Instance.ResourceManager.Destroy(gameObject, true);
            }
        }

        public override bool Drop(in Vector2 parentLinVelocity)
        {
            bool ret = base.Drop(parentLinVelocity);
            StopAttack();
            return ret;
        }

        public override bool Throw(in Vector2 dir, in Vector2 parentLinVelocity, float force, int chargeRate)
        {
            bool ret = base.Throw(dir, parentLinVelocity, force, chargeRate);
            StopAttack();
            return ret;
        }


        private void Update()
        {
            if (_isPressed && _cooldownModule.IsCooldownEnded())
            {
                _cooldownModule.StartCooldown();
                ProjectileLaunchHelper.LaunchConstantDir(_attackableLayers, _bullectProjectileIDX, transform.position, transform.right);
                _weaponUseCnt--;
                if(CanUseWeapon() == false)
                {
                    StopAttack();
                }
            }
        }

        public override void OnLMBPressed()
        {
            _isPressed =true;
        }

        public override void OnLMBReleased()
        {
            _isPressed =false;
        }

        public override void StopAttack()
        {
            _isPressed = false;
        }
    }
}
