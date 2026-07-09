using System;
using UnityEngine;

namespace Coordinator.Victims
{
    public class ButtonActivateor : MonoBehaviour, IAttackable
    {
        private int _maskedLayer = 0;
        private ButtonCoordinator _original;
        private bool _isAttackableOn;
        private void Start()
        {
            _isAttackableOn = true;
            _original = transform.parent.GetComponent<ButtonCoordinator>();
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

        public void TakeDamage(int damage)
        {
            _original.SetState(true);
        }

        public int GetMaskedLayer()
        {
            return _maskedLayer;
        }
        public bool CanAttack()
        {
            return _isAttackableOn && _original.GetState()==false;
        }
        #region UnusedFuncs
        [Obsolete]
        public T RequestComponent<T>() where T : class
        {
            return null;
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
