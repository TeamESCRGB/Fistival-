using ComponentModule;
using Coordinator.Objects;
using Coordinator.Objects.Weapons;
using Data;
using Defines;
using Manager;
using System;
using UnityEditor;
using UnityEngine;
using static Utils.VectorUtils;

namespace Coordinator
{
    public abstract class HandCoordinatorBase : MonoBehaviour
    {
        #region AboutHandStat
        protected HandStatus _status = HandStatus.IDLE;
        protected Vector2 _mousePos = Vector2.zero;
        protected Camera _mainCam;
        protected LayerMask _attackableMask = 0;
        protected Rigidbody2D _parentRb2d;
        protected ObjectCoordinator _grabbedObject;
        [SerializeField]protected WeaponCoordinatorBase _weapon;
        protected CooldownComponentModule _cooldownModule;
        #endregion

        #region AboutPickup
        [SerializeField]
        protected Vector2 _pickupBoxcastSize;
        [SerializeField]
        protected float _pickupBoxcastDistance;
        protected LayerMask _pickableObjectMask;
        protected Transform _handAnchor;
        protected (GameObject obj, TargetObjectHighlighter highlighter) _nowSelectedObject;
        #endregion

        #region AboutCharge
        protected int _maxChargeCnt = 3;
        protected int _chargeCnt = 0;
        protected float _forcePerCharge = 10;
        protected float _chargeTimeInterval = 1;
        protected float _chargeTime = 0;
        public event Action<int, int> OnChargeRateChanged;//now rate, max rate
        public event Action<ObjectData> OnGrabbedObjectChanged;
        #endregion

        #region Events

        protected void ResetEvents()
        {
            OnChargeRateChanged = null;
            OnGrabbedObjectChanged = null;
        }

        protected void InvokeOnChargeRateChanged(int nowRate, int maxRate)
        {
            OnChargeRateChanged?.Invoke(nowRate,maxRate);
        }

        protected void InvokeOnGrabbedObjectChanged(ObjectData data)
        {
            OnGrabbedObjectChanged?.Invoke(data);
        }

        #endregion

        protected void InitCommonDatas(Rigidbody2D parentRb2d, LayerMask attackableMask,LayerMask pickableObjectMask, float forcePerCharge, float chargeTimeInterval, float attackCooldown)
        {
            _pickableObjectMask = pickableObjectMask;
            _attackableMask = attackableMask;
            _status = HandStatus.IDLE;
            _grabbedObject = null;
            _weapon = null;
            _parentRb2d = parentRb2d;
            _chargeTime = 0;
            _chargeCnt = 0;
            _forcePerCharge = forcePerCharge;
            _chargeTimeInterval = chargeTimeInterval;
            _cooldownModule = Managers.Instance.CooldownManager.GetCooldownModule(attackCooldown, 0.1f);
            _nowSelectedObject = (null, null);
        }


        public void SetMousePos(in Vector2 mousePos)
        {
            _mousePos = mousePos;
        }

        public HandStatus GetNowHandStatus()
        {
            return _status;
        }


        public int GetMaxCharge()
        {
            return _maxChargeCnt;
        }

        public void SetMaxCharge(int maxCharge)
        {
            if (maxCharge < 0)
            {
                maxCharge = 0;
            }
            _maxChargeCnt = maxCharge;
        }

        #region LifecycleFunctionInheritance

        private void Update()
        {
            if(Managers.Instance.GameManager.IsGamePaused())
            {
                return;
            }
            OnUpdate();
        }

        private void Awake()
        {
            OnAwake();
        }

        private void Start()
        {
            OnStart();
        }
        private void OnDisable()
        {
            OnDisabled();
        }

        protected virtual void OnUpdate()
        {
            if (_status == HandStatus.CHARGE && _chargeCnt < _maxChargeCnt)
            {
                _chargeTime += Time.deltaTime;

                if (_chargeTime >= _chargeTimeInterval)
                {
                    _chargeTime = 0;
                    _chargeCnt += 1;
                    InvokeOnChargeRateChanged(_chargeCnt, _maxChargeCnt);
                }
            }
        }
        protected virtual void OnAwake()
        {
            _mainCam = Camera.main;
            _handAnchor = transform.Find("@HandAnchor");
#if UNITY_EDITOR
            if (_handAnchor == null)
            {
                Debug.LogError($"@HandAnchor 가 {gameObject.name}의 자식중에 없습니다.");
            }

            if (_mainCam == null)
            {
                Debug.LogError($"MainCamera테그를 가진 카메라가 없습니다.");
            }
#endif
        }
        protected virtual void OnStart() { }
        protected virtual void OnDisabled()
        {
            if(Managers.Instance != null && _cooldownModule is not null)
            {
                Managers.Instance.CooldownManager.ReturnModule(_cooldownModule);
            }
            _cooldownModule = null;
        }
        #endregion

        #region PickupOperations

        private (GameObject, TH) UpdateSelectedObjectState<TH>(LayerMask targetObjectMask,(GameObject obj,TH highlighter) result) where TH : TargetObjectHighlighter
        {
            var hit = Physics2D.BoxCast(_handAnchor.position, _pickupBoxcastSize, 0, _handAnchor.right, _pickupBoxcastDistance, targetObjectMask);

            if (hit.collider == null)
            {
                if (result.obj == null)
                {
                    return result;
                }

                if (result.highlighter != null)
                {
                    result.highlighter.DeActivateShader();
                }
                return (null, null);
            }
            else
            {
                if (hit.collider.gameObject == result.obj)
                {
                    return result;
                }

                if (result.highlighter != null)
                {
                    result.highlighter.DeActivateShader();
                }

                var highlighter = hit.collider.gameObject.GetComponent<TH>();
                highlighter.ActivateShader();
                return (hit.collider.gameObject, highlighter);
            }
        }

        private void UpdateTargetObjectState()
        {
            _nowSelectedObject = UpdateSelectedObjectState<TargetObjectHighlighter>(_pickableObjectMask,_nowSelectedObject);
        }


        private void FixedUpdate()
        {
            if(Managers.Instance.GameManager.IsGamePaused())
            {
                return;
            }
            UpdateTargetObjectState();
        }
        #endregion

        #region WeaponOperations
        protected bool CanUseWeapon()
        {
            return _weapon != null && _weapon.CanUseWeapon();
        }

        protected void RemoveWeapon()
        {
            _weapon = null;
            //애니메이션 컨트롤러 복구
        }
        #endregion

        #region RMBOperations

        public void StopCharging()
        {
            _status = HandStatus.GRABBED;
            _chargeCnt = 0;
            InvokeOnChargeRateChanged(_chargeCnt, _maxChargeCnt);
        }

        protected virtual void Throw()
        {
            _status = HandStatus.IDLE;
            _grabbedObject.SetAttackableLayer(_attackableMask);
            _grabbedObject.Throw(GetDirVec2(_mainCam.ScreenToWorldPoint(_mousePos), _handAnchor.position), _parentRb2d.linearVelocity, _forcePerCharge * _chargeCnt, _chargeCnt);
            _chargeCnt = 0;
            _grabbedObject = null;
            RemoveWeapon();
            InvokeOnChargeRateChanged(_chargeCnt, _maxChargeCnt);
            InvokeOnGrabbedObjectChanged(null);
        }
        protected virtual void Pickup()
        {
            UpdateTargetObjectState();
            _status = HandStatus.IDLE;
            if(_nowSelectedObject.obj != null)
            {
                _nowSelectedObject.highlighter.DeActivateShader();
                _grabbedObject = _nowSelectedObject.obj.GetComponent<ObjectCoordinator>();
                _grabbedObject.PickUp(_handAnchor);
                InvokeOnGrabbedObjectChanged(_grabbedObject.GetSharedData());
                _status = HandStatus.GRABBED;
                _nowSelectedObject = (null, null);

                if(_grabbedObject.TryGetComponent<WeaponCoordinatorBase>(out var weapon) && weapon.CanUseWeapon())
                {
                    _weapon = weapon;
                    _weapon.SetAttackableLayer(_attackableMask);
                    //기존 애니메이션 컨틀롤러 백업
                    //애니메이션 컨틀롤러 덮어쓰기
                }
            }
            //var hit = Physics2D.BoxCast(_handAnchor.position, _pickupBoxcastSize, 0, _handAnchor.right, _pickupBoxcastDistance, _pickableObjectMask);
            //if (hit.transform != null && hit.transform.gameObject.TryGetComponent<ObjectCoordinator>(out var comp))
            //{
            //    _grabbedObject = comp;
            //    comp.PickUp(_handAnchor);
            //    InvokeOnGrabbedObjectChanged(comp.GetSharedData());
            //    _status = HandStatus.GRABBED;
            //}
        }
        public virtual void Drop()
        {
            if (_grabbedObject != null)
            {
                _status = HandStatus.IDLE;
                _grabbedObject.Drop(_parentRb2d.linearVelocity);
                _grabbedObject = null;
                RemoveWeapon();
                _chargeCnt = 0;
                InvokeOnChargeRateChanged(_chargeCnt, _maxChargeCnt);
                InvokeOnGrabbedObjectChanged(null);
            }
        }

        public virtual void OnRMBPressed()
        {
            if (_grabbedObject == null)
            {
                Pickup();
            }
            else
            {
                _chargeTime = 0;
                _chargeCnt = 1;
                _status = HandStatus.CHARGE;
            }
        }

        public virtual void OnRMBReleased()
        {
            if (_status == HandStatus.CHARGE && _grabbedObject != null)
            {
                Throw();
            }
        }

        #endregion

        #region LMBOperations

        public virtual void OnLMBPressed()
        {
            _weapon.OnLMBPressed();
            if(CanUseWeapon() == false)
            {
                RemoveWeapon();
            }
        }
        public virtual void OnLMBReleased()
        {
            _weapon.OnLMBReleased();
            if (CanUseWeapon() == false)
            {
                RemoveWeapon();
            }
        }

        #endregion
    }
}