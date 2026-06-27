using Coordinator.Victims;
using System;
using UnityEngine;

namespace Coordinator
{
    public abstract class SkillCoordinatorBase : MonoBehaviour
    {
        protected int _attackableLayers = 0;

        protected int _baseDamage = 0;

        protected float _baseStunTime=0;

        private event Action<int, int> _onAttack;

        public void Init(int attackableLayers, int baseDamage, float baseStunTime)
        {
            _baseDamage = baseDamage;
            _attackableLayers = 0;
            _baseStunTime=baseStunTime;
            SetAttackableLayer(attackableLayers);
        }

        protected void ResetOnAttack()
        {
            _onAttack = null;
        }

        public void RegisterOnAttack(Action<int,int> callback)
        {
            _onAttack -= callback;
            _onAttack += callback;
        }

        public void UnRegisterOnAttack(Action<int,int> callback)
        {
            _onAttack -= callback;
        }

        protected void CallOnAttack(int attackCnt, int attackData)
        {
            _onAttack?.Invoke(attackCnt,attackData);
        }

        public void SetAttackableLayer(int attackableLayers)
        {
            _attackableLayers = attackableLayers;
        }

        public virtual bool CanAttackTarget(IAttackable target)
        {
            return ((target.GetMaskedLayer() & _attackableLayers) != 0) && target.CanAttack();
        }

        public int GetBaseDamage => _baseDamage;
        public virtual void SetBaseDamage(int damage)
        {
            _baseDamage = damage;
        }

        public abstract bool Act(IAttackable target, int calculatedDamage, Vector2 knockback);
    }
}