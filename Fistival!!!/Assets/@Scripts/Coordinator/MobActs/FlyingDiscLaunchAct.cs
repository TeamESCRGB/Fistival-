using Coordinator.Objects;
using Data;
using Manager;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Coordinator.MobActs
{
    public class FlyingDiscLaunchAct : MobActBase
    {
        private int _objDataIdx;
        private string _objKey;
        private int _damage;
        private float _stunTime;
        private float _duration;
        private float _movLen;
        private LayerMask _objectizableLayer;
        [SerializeField]
        private LayerMask _objectLayer;
        private LayerMask _playerHitboxLayer;
        private Transform _grabBox;

        private bool _isActing;

        private int _returnCode;

        private void Awake()
        {
            _grabBox = transform.Find("@FlyingdiscGrabBox");
        }

        public void Init(Action onEnd, Animator animator, string objKey, int objDataIdx ,int damage, float stunTime, float returnDuration, float movLen, LayerMask objectizableLayer, LayerMask playerHitboxLayer)
        {
            base.Init(onEnd,animator);
            _playerHitboxLayer= playerHitboxLayer;
            _isActing = false;
            _returnCode = -666775;
            _objDataIdx = objDataIdx;
            _objKey = objKey;
            _damage=damage;
            _stunTime=stunTime;
            _duration=returnDuration;
            _movLen=movLen;
            _objectizableLayer = objectizableLayer;
        }

        private void OnObjectized()
        {
            _isActing = false;
            _returnCode = -666775;
            _onActEnd?.Invoke();
        }

        private void OnReturnStart()
        {
            _isActing = true;
        }

        private void FixedUpdate()
        {
            if(_isActing == false)
            {
                return;
            }

            var targets = Physics2D.OverlapBoxAll(_grabBox.position, _grabBox.localScale, 0, _objectLayer);

            for(int i = 0; i < targets.Length; i++)
            {
                if (targets[i].gameObject.GetInstanceID() == _returnCode)
                {
                    Managers.Instance.ResourceManager.Destroy(targets[i].gameObject);
                    End();
                    return;
                }
            }
        }

        public void Launch()
        {
            if(Managers.Instance.DataManager.ObjectDataDict.TryGetValue(_objDataIdx, out var data) == false)
            {
                return;
            }
            var go = Managers.Instance.ResourceManager.Instantiate(_objKey, null, true, true);
            if(go == null)
            {
                return;
            }
            go.GetComponent<ObjectCoordinator>().Init(data);
            go.GetComponent<ObjectCoordinator>().SetAttackableLayer(_playerHitboxLayer);
            go.transform.position = transform.position;
            var attackToObj = go.GetComponent<AttackToObject>();
            attackToObj.Init(_duration, _movLen, _objectizableLayer, _damage, _stunTime);
            attackToObj.OnObjectized += OnObjectized;
            attackToObj.OnReturnStart += OnReturnStart;
            _returnCode = go.GetInstanceID();
            attackToObj.Launch();
        }

        public void End()
        {
            _isActing = false;
            _onActEnd?.Invoke();
        }

        public override void Act()
        {
            Launch();
        }

        public override void StopAct()
        {

        }
    }
}
