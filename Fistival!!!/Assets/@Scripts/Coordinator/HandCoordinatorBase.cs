using Coordinator.Objects;
using Data;
using Defines;
using System;
using UnityEngine;

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
        #endregion

        #region AboutPickup
        [SerializeField]
        protected Vector2 _pickupBoxcastSize;
        [SerializeField]
        protected float _pickupBoxcastDistance;
        [SerializeField]
        protected LayerMask _pickableObjectMask;
        protected Transform _handAnchor;
        #endregion

        #region AboutCharge
        protected int _maxChargeCnt = 3;
        protected int _chargeCnt = 0;
        [SerializeField]
        protected float _forcePerCharge = 10;
        [SerializeField]
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

        protected virtual void OnUpdate() { }
        protected virtual void OnAwake() { }
        protected virtual void OnStart() { }

        #endregion


        #region RMBOperations

        protected abstract void Throw();
        protected abstract void Pickup();
        public abstract void Drop();

        public abstract void OnRMBPressed();

        public abstract void OnRMBReleased();

        #endregion

        #region LMBOperations

        public abstract void OnLMBPressed();
        public abstract void OnLMBReleased();

        #endregion
    }
}