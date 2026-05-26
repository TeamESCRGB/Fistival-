using Coordinator.Movements;
using Defines;
using UnityEngine;

namespace Coordinator.Chain
{
    public class ChainMorningStar : MonoBehaviour
    {
        [SerializeField]
        private float _strongDamageMultiplier;
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
        public void Init(LayerMask attackableMask, Transform parentTransform, int damage, IChainPullable player)
        {
            _parentTransform = parentTransform;
            _anchor.Init(attackableMask, _pullTotalTime, player);
            _baseDamage = damage;
        }

        private void FixedUpdate()
        {
            transform.position = _parentTransform.position;
        }

        public void SetRotation(Vector2 dir)
        {
            if(_anchor.GetStatus() != ChainStatus.OFF)
            {
                return;
            }
            transform.localRotation = Quaternion.Euler(0, 0, Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg);
        }

        public void Launch(Vector2 dir, AttackStatus status)
        {
            if(_anchor.GetStatus() != ChainStatus.OFF)
            {
                return;
            }

            float len = _baseMaxLength;
            int damage = _baseDamage;
            if(status == AttackStatus.STRONG)
            {
                len *= 2;
                damage = (int)(damage * _strongDamageMultiplier);
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