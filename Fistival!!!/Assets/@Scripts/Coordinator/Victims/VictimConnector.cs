using UnityEngine;

namespace Coordinator.Victims
{
    public class VictimConnector : MonoBehaviour, IAttackable
    {
        private IAttackable _original;
        private int _maskedLayer = 0;

        public void SetAttackableState(bool canAttack)
        {
            _original.SetAttackableState(canAttack);
        }

        public bool IsAttackableStateOn()
        {
            return _original.IsAttackableStateOn();
        }

        public void SetOriginal(IAttackable original)
        {
            _original = original;
            _maskedLayer = 1 << gameObject.layer;
        }

        public bool CanAttack()
        {
            return _original.CanAttack();
        }

        public int GetMaskedLayer()
        {
            return _maskedLayer;
        }

        public T RequestComponent<T>() where T : class
        {
            return _original.RequestComponent<T>();
        }

        public void StartInvincibleTime()
        {
            _original.StartInvincibleTime();
        }

        public void TakeDamage(int damage)
        {
            _original.TakeDamage(damage);
        }

        public void TakeKnockBack(Vector2 force)
        {
            _original.TakeKnockBack(force);
        }

        public void StunFor(float time)
        {
            _original.StunFor(time);
        }

        public void ReleaseStun()
        {
            _original.ReleaseStun();
        }
    }
}