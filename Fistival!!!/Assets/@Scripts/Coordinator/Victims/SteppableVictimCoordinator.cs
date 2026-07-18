using UnityEngine;

namespace Coordinator.Victims
{
    public class SteppableVictimCoordinator : MonoBehaviour, IAttackable
    {
        private IAttackable _original;
        private int _maskedLayer = 0;
        private Collider2D _collider;

        private void Awake()
        {
            _collider = GetComponent<Collider2D>();
            _original = transform.parent.gameObject.GetComponent<IAttackable>();
            _maskedLayer = 1<<gameObject.layer;

#if UNITY_EDITOR
            if(_original == null)
            {
                Debug.LogError($"SteppableVictimCoordinator가 붙은 {transform.parent.gameObject.name}에 IAttackable이 없음");
            }
#endif
        }
        public void SetAttackableState(bool canAttack)
        {
            _original.SetAttackableState(canAttack);
        }

        public bool IsAttackableStateOn()
        {
            return _original.IsAttackableStateOn();
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

        public void TakeDamage(int damage, Vector3 attackerPos, bool showHitEffect)
        {
            _original.TakeDamage(damage,attackerPos,false);
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