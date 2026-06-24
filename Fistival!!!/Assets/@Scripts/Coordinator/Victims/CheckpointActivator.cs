using Coordinator.Interactables;
using UnityEngine;

namespace Coordinator.Victims
{
    public class CheckpointActivator : MonoBehaviour, IAttackable
    {
        private int _maskedLayer = 0;
        private CheckpointInteractor _checkpoint;
        private void Start()
        {
            _checkpoint = transform.parent.GetComponent<CheckpointInteractor>();
            _maskedLayer = 1 << gameObject.layer;
        }

        public void TakeDamage(int damage)
        {
            _checkpoint.CheckCheckpoint(true);
        }

        public int GetMaskedLayer()
        {
            return _maskedLayer;
        }
        public bool CanAttack()
        {
            return _checkpoint.IsCheckpointChecked() == false;
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
        #endregion
    }
}
