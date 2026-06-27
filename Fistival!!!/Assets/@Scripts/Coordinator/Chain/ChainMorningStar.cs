using Coordinator.Movements;
using Defines;
using Manager;
using UnityEngine;

namespace Coordinator.Chain
{
    public class ChainMorningStar : MonoBehaviour
    {
        protected int _strongAttackDamage;
        [SerializeField]
        private float _baseMaxLength;
        [SerializeField]
        private float _totalMoveTime;
        private ChainAnchor _anchor;
        private Transform _parentTransform;
        private int _baseDamage;

        [SerializeField]
        private float _pullTotalTime=0.7f;

        private void Awake()
        {
            _anchor = GetComponentInChildren<ChainAnchor>();
        }
        public void Init(LayerMask attackableMask, Transform parentTransform, int damage,int strongAttackDamage ,IChainPullable player, float stunTime)
        {
            _parentTransform = parentTransform;
            _anchor.Init(attackableMask, _pullTotalTime, player);
            _baseDamage = damage;
            _strongAttackDamage = strongAttackDamage;
        }

        private void FixedUpdate()
        {
            if (Managers.Instance.GameManager.IsGamePaused())
            {
                return;
            }
            transform.position = _parentTransform.position;
        }

        public void SetRotation(Vector2 dir)
        {
            if(_anchor.IsMoving())
            {
                return;
            }
            transform.localRotation = Quaternion.Euler(0, 0, Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg);
        }

        public void SetDamage(int damage)
        {
            _baseDamage = damage;
        }

        public void SetStrongDamage(int damage)
        {
            _strongAttackDamage = damage;
        }

        public void Launch(Vector2 dir, AttackStatus status)
        {
            if(_anchor.IsMoving())
            {
                return;
            }

            float len = _baseMaxLength;
            int damage = _baseDamage;
            if(status == AttackStatus.STRONG)
            {
                len *= 2;
                damage += _strongAttackDamage;
            }

            SetRotation(dir);
            _anchor.Launch(dir,len,_totalMoveTime,damage);
        }

        public void Retrive()
        {
            _anchor.Retrive();
        }
    }
}