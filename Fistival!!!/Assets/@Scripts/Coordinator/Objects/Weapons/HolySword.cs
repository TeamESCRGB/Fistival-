using Coordinator.Skills;
using Coordinator.Victims;
using Data;
using Manager;
using UnityEngine;

namespace Coordinator.Objects.Weapons
{
    public class HolySword : WeaponCoordinatorBase
    {
        private Transform _attackBox;
        private bool _isPressed = false;
        [SerializeField]
        private int _damage;

        protected override void OnAwake()
        {
            base.OnAwake();
            _attackBox = transform.Find("@AttackBox");

#if UNITY_EDITOR
            Debug.Assert(_attackBox != null, $"{name} 의 자식중에 @AttackBox가 없음");
#endif
        }
        public override void Init(ObjectData data)
        {
            base.Init(data);
            _isPressed = false;
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

        public override void OnLMBPressed()
        {
            _isPressed = true;
            _smashedEnemyCnt = 0;
        }

        public override void OnLMBReleased()
        {
            if (_isPressed == false)
            {
                return;
            }


            var enemies = Physics2D.OverlapBoxAll(_attackBox.position, _attackBox.localScale, 0, _attackableLayers);


            if (enemies is null)
            {
                return;
            }

            Vector2 knockback = new Vector2(_attackBox.forward.z * _damage, 0);

            for (int i = 0; i < enemies.Length; i++)
            {
                Collider2D enemy = enemies[i];
                if (enemy.gameObject.TryGetComponent<IAttackable>(out var comp) == false)
                {
                    continue;
                }
                Managers.Instance.AttackManager.RequestAttack(comp, this, _damage, knockback,_baseStunTime);
            }


            if (enemies.Length > 0)
            {
                _weaponUseCnt--;
            }
            if (CanUseWeapon() == false)
            {
                StopAttack();
            }
            _isPressed = false;
        }

        public override void StopAttack()
        {
            _isPressed = false;
        }
    }
}