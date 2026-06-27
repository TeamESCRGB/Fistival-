using Coordinator.Movements;
using UnityEngine;

namespace Coordinator.Victims
{
    public interface IAttackable : IStunnable
    {
        public bool CanAttack();
        public T RequestComponent<T>() where T : class;
        public void TakeDamage(int damage);
        public void StartInvincibleTime();
        public int GetMaskedLayer();
        public void TakeKnockBack(Vector2 force);
    }
}