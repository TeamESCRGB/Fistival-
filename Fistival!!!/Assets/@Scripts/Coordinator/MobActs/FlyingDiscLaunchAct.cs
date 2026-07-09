using Coordinator.Objects;
using Manager;
using System;
using UnityEngine;

namespace Coordinator.MobActs
{
    public class FlyingDiscLaunchAct : MobActBase
    {
        private int _objDataIdx;
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
        private bool _isReturning;

        private AttackToObject _obj;

        private int _returnCode;

        private void Awake()
        {
            _grabBox = transform.Find("@FlyingdiscGrabBox");
        }

        public void Init(Action onEnd, Animator animator, int objDataIdx ,int damage, float stunTime, float returnDuration, float movLen, LayerMask objectizableLayer, LayerMask playerHitboxLayer)
        {
            base.Init(onEnd,animator);
            _obj = null;
            _playerHitboxLayer= playerHitboxLayer;
            _isActing = false;
            _isReturning = false;
            _returnCode = -666775;
            _objDataIdx = objDataIdx;
            _damage=damage;
            _stunTime=stunTime;
            _duration=returnDuration;
            _movLen=movLen;
            _objectizableLayer = objectizableLayer;
        }

        private void OnObjectized()
        {
            if(_isActing)
            {
                _obj = null;
                _isReturning = false;
                _isActing = false;
                _returnCode = -666775;
                _onActEnd?.Invoke();
            }
        }

        private void OnReturnStart()
        {
            if(_isActing)
            {
                _isReturning = true;
            }
        }

        private void FixedUpdate()
        {
            if(_isReturning == false)
            {
                return;
            }

            var targets = Physics2D.OverlapBoxAll(_grabBox.position, _grabBox.localScale, _grabBox.rotation.z, _objectLayer);

            for(int i = 0; i < targets.Length; i++)
            {
                if (targets[i].gameObject.GetInstanceID() == _returnCode)
                {
                    Managers.Instance.ResourceManager.Destroy(targets[i].gameObject);
                    EndFlyingDisc();
                    return;
                }
            }
        }

        public void LaunchFlyingDisc()
        {
            if(Managers.Instance.DataManager.ObjectDataDict.TryGetValue(_objDataIdx, out var data) == false)
            {
                return;
            }
            var go = Managers.Instance.ResourceManager.Instantiate(data.PrefabKey, null, true, true);
            if(go == null)
            {
                return;
            }
            
            go.GetComponent<ObjectCoordinator>().Init(data);
            go.GetComponent<ObjectCoordinator>().SetAttackableLayer(_playerHitboxLayer);
            go.transform.position = transform.position;
            _obj = go.GetComponent<AttackToObject>();
            _obj.Init(_duration, _movLen, _objectizableLayer, _damage, _stunTime);
            _obj.OnObjectized += OnObjectized;
            _obj.OnReturnStart += OnReturnStart;
            _returnCode = go.GetInstanceID();
            _obj.Launch();
        }

        public void EndFlyingDisc()
        {
            if(_isActing)
            {
                _isReturning = false;
                _isActing = false;
                _obj = null;
                _animator.SetTrigger("Catch");
                _onActEnd?.Invoke();
            }
        }

        public override void Act()
        {
            _isActing = true;
            _animator.SetTrigger("Launch");//Launch();
        }

        public override void StopAct()
        {
            if (_isActing)
            {
                _isReturning = false;
                _isActing = false;
                if(_obj != null)
                {
                    _obj.OnObjectized -= OnObjectized;
                    _obj.OnReturnStart -= OnReturnStart;
                }
                _obj = null;
                _onActEnd?.Invoke();
            }
        }
    }
}
