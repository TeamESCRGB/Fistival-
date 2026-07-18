using Coordinator.Movements;
using UnityEngine;

namespace Coordinator.Victims
{
    public interface IAttackable : IStunnable
    {
        public bool CanAttack();
        public T RequestComponent<T>() where T : class;
        public void TakeDamage(int damage, Vector3 attackerPos);
        public void StartInvincibleTime();
        public int GetMaskedLayer();
        public void SetAttackableState(bool canAttack);
        public bool IsAttackableStateOn();
        public void TakeKnockBack(Vector2 force);
    }
}