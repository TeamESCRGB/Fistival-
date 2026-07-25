using Coordinator.Victims;
using Manager;
using UnityEngine;

namespace Coordinator.Skills
{
    [RequireComponent(typeof(TouchDamageSkill))]
    public class StandaloneTouchDamage : TouchDamageSkill
    {
        [SerializeField]
        protected Vector2 _knockbackDir;
        private Vector2 _knockbackDirNorm;
        [SerializeField]
        private LayerMask _attackables_ser;
        [SerializeField]
        private int _damage_ser;
        [SerializeField]
        private float _stunTime_ser;
        [SerializeField]
        private float _knockBackForce_ser;

        [SerializeField]
        private bool _useBoxOverlap_se;
        [SerializeField]
        private bool _useBoxCollider_se;

        private void Start()
        {
            _useOverlapBox = _useBoxOverlap_se;
            _usePhys2d = _useBoxCollider_se;
            _knockbackDirNorm = _knockbackDir.normalized;
            Init(_attackables_ser, _damage_ser, _stunTime_ser, _knockBackForce_ser, true,false);
        }

        protected override void Attack(GameObject go)
        {
            if (go == null)
            {
                return;
            }

            if (((1 << go.layer) & _attackableLayers) == 0 || go.TryGetComponent<IAttackable>(out var comp) == false)
            {
                return;
            }

            var knockBackForce = _baseDamage * _knockbackForce;
            var force = _knockbackDirNorm * knockBackForce;
            Managers.Instance.AttackManager.RequestAttack(comp, this, _baseDamage, force, _baseStunTime);
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;

            Vector3 knockbackVector = new Vector3(_knockbackDirNorm.x, _knockbackDirNorm.y, 0f) * _knockBackForce_ser;

            Gizmos.DrawRay(transform.position, knockbackVector);
        }
#endif
    }
}