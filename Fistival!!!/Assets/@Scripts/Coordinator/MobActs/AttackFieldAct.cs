using System;
using UnityEngine;
using Utils;

namespace Coordinator.MobActs
{
    public class AttackFieldAct : MobActBase
    {
        [SerializeField]
        private string _attackFieldName;
        private GameObject _attackField;
        private bool _isActing;
        private void Awake()
        {
            _attackField = gameObject.GetChildGameObject(_attackFieldName,true,true);
        }
        public override void Init(Action onActionEnd, Animator animator)
        {
            base.Init(onActionEnd, animator);
            _attackField.SetActive(false);
            _isActing = false;
        }

        public void OnAttackFieldActEnd()
        {
            _attackField.SetActive(false);
            _isActing = false;
            _onActEnd?.Invoke();
        }

        public void OnAttackFieldActStart()
        {
            _attackField.SetActive(true);
        }

        public override void Act()
        {
            _isActing = true;
            _animator.SetTrigger("ActivateAttackField");
        }

        public override void StopAct()
        {
            if(_isActing == false)
            {
                return;
            }
            _attackField.SetActive(false);
            _isActing = false;
            _onActEnd?.Invoke();
        }
    }
}
