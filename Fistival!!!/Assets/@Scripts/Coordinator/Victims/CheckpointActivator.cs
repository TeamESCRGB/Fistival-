using Coordinator.Interactables;
using UnityEngine;

namespace Coordinator.Victims
{
    public class CheckpointActivator : MonoBehaviour, IAttackable
    {
        private int _maskedLayer = 0;
        private CheckpointInteractor _checkpoint;
        private bool _isAttackableOn;
        private Collider2D _collider;
        private void Awake()
        {
            _collider = GetComponent<Collider2D>();
        }
        private void Start()
        {
            _isAttackableOn = true;
            _checkpoint = transform.parent.GetComponent<CheckpointInteractor>();
            _maskedLayer = 1 << gameObject.layer;
        }

        public void SetAttackableState(bool canAttack)
        {
            _isAttackableOn = canAttack;
        }
        public bool IsAttackableStateOn()
        {
            return _isAttackableOn;
        }

        public void TakeDamage(int damage, Vector3 attackerPos, bool showHitEffect)
        {
            _checkpoint.CheckCheckpoint(true);
        }

        public int GetMaskedLayer()
        {
            return _maskedLayer;
        }
        public bool CanAttack()
        {
            return _isAttackableOn && _checkpoint.IsCheckpointChecked() == false;
        }
        #region UnusedFuncs
        public T RequestComponent<T>() where T : class
        {
            return GetComponent<T>();
        }
        
        public void StartInvincibleTime()
        {

        }

        public void TakeKnockBack(Vector2 force)
        {

        }

        public void StunFor(float time)
        {

        }

        public void ReleaseStun()
        {

        }
        #endregion
    }
}
