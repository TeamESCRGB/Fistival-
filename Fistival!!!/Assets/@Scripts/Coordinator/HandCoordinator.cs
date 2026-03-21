using Coordinator.Victims;
using Data;
using Defines;
using Manager;
using System;
using UnityEngine;
using Utils;
using static Utils.VectorUtils;

namespace Coordinator
{
    public class HandCoordinator : MonoBehaviour
    {
        [SerializeField]
        private Vector2 _pickupBoxcastSize;
        [SerializeField]
        private float _pickupBoxcastDistance;
        [SerializeField]
        private LayerMask _pickableObjectMask;
        private Transform _handAnchor;
        private Transform _attackBox;
        private ObjectCoordinator _grabbedObject;
        private Rigidbody2D _parentRb2d;

        private int _maxChargeCnt = 3;
        private int _chargeCnt = 0;

        [SerializeField]
        private float _forcePerCharge=10;
        [SerializeField]
        private float _chargeTimeInterval=1;
        private float _chargeTime=0;

        public Vector2 MousePos { get; set; }


        [SerializeField]private HandStatus _status = HandStatus.IDLE;

        public event Action<int,int> OnChargeRateChanged;//now rate, max rate
        public event Action<ObjectData> OnGrabbedObjectChanged;

        private int _baseSmashDamage;

        private LayerMask _attackableMask=0;

        private SkillCoordinatorBase _skillBase;

        private Camera _mainCam;

        private void Awake()
        {
            _handAnchor = transform.Find("@HandAnchor");
            _attackBox = transform.Find("@AttackBox");
            _mainCam = Camera.main;

#if UNITY_EDITOR
            if (_handAnchor == null)
            {
                Debug.LogError($"@HandAnchor 가 {gameObject.name}의 자식중에 없습니다.");
            }

            if (_attackBox == null)
            {
                Debug.LogError($"@AttackBox 가 {gameObject.name}의 자식중에 없습니다.");
            }

            if (_mainCam == null)
            {
                Debug.LogError($"MainCamera테그를 가진 카메라가 없습니다.");
            }
#endif

            _skillBase = _attackBox.gameObject.GetComponent<SkillCoordinatorBase>();
#if UNITY_EDITOR
            if (_skillBase == null)
            {
                Debug.LogError($"{_attackBox.name} 에 skillbase가 없습니다");

            }
#endif
            
        }

        private void Update()
        {
            //게임 일시정지 로직 나중에 추가
            if(_status == HandStatus.CHARGE && _chargeCnt < _maxChargeCnt)
            {
                _chargeTime += Time.deltaTime;

                if(_chargeTime >= _chargeTimeInterval)
                {
                    _chargeTime = 0;
                    _chargeCnt += 1;
                    OnChargeRateChanged?.Invoke(_chargeCnt,_maxChargeCnt);
                }
            }
        }

        public int GetMaxCharge()
        {
            return _maxChargeCnt;
        }

        public void SetMaxCharge(int maxCharge)
        {
            if(maxCharge < 0)
            {
                maxCharge = 0;
            }
            _maxChargeCnt = maxCharge;
        }

        

        public void Init(Rigidbody2D parentRb2d, int baseSmashDamage, LayerMask attackableFilter)
        {
            _status = HandStatus.IDLE;
            _attackableMask = attackableFilter;
            _grabbedObject = null;
            _parentRb2d = parentRb2d;
            _chargeTime = 0;
            _chargeCnt = 0;
            OnChargeRateChanged = null;
            OnGrabbedObjectChanged = null;
            _baseSmashDamage = baseSmashDamage;

            _skillBase.Init(_attackableMask,_baseSmashDamage);
        }

        public void Drop()
        {
            if(_grabbedObject != null)
            {
                _status = HandStatus.IDLE;
                _grabbedObject.Drop(_parentRb2d.linearVelocity);
                _grabbedObject = null;
                _chargeCnt = 0;
                OnChargeRateChanged?.Invoke(_chargeCnt,_maxChargeCnt);
                OnGrabbedObjectChanged?.Invoke(null);
            }        
        }

        public void Attack()
        {
            var enemy = Physics2D.OverlapBox(_attackBox.position, _attackBox.localScale, 0, _attackableMask);

            if (enemy == null || enemy.gameObject.TryGetComponent<IAttackable>(out var comp) == false)
            {
                return;
            }

            //BoxOverlap에 필터링에 걸린것만 가져와서 수행.
            //없으면 실행 안함
            int totalDmg = _baseSmashDamage;
            if (_grabbedObject != null)
            {

                totalDmg += _grabbedObject.GetSharedData().Damage;

                if (_grabbedObject.Smash() == false)
                {
                    _grabbedObject = null;
                    _chargeCnt = 0;
                    OnChargeRateChanged?.Invoke(_chargeCnt, _maxChargeCnt);
                    OnGrabbedObjectChanged?.Invoke(null);
                    _status = HandStatus.IDLE;
                }
            }

            Managers.Instance.AttackManager.RequestAttack(comp, _skillBase, totalDmg);
            //공격하는 함수. 데미지: damage +  해서 OverlapBox써서 나중에 함
        }

        private void Throw()
        {
            _status = HandStatus.IDLE;
            _grabbedObject.SetAttackableLayer(_attackableMask);
            _grabbedObject.Throw(GetDirVec2(_mainCam.ScreenToWorldPoint(MousePos), _handAnchor.position), _parentRb2d.linearVelocity, _forcePerCharge * _chargeCnt);
            _chargeCnt = 0;
            _grabbedObject = null;
            OnChargeRateChanged?.Invoke(_chargeCnt, _maxChargeCnt);
            OnGrabbedObjectChanged?.Invoke(null);
        }

        private void Pickup()
        {
            _status = HandStatus.IDLE;
            var hit = Physics2D.BoxCast(_handAnchor.position, _pickupBoxcastSize, 0, _handAnchor.right, _pickupBoxcastDistance, _pickableObjectMask);
            if (hit.transform != null && hit.transform.gameObject.TryGetComponent<ObjectCoordinator>(out var comp))
            {
                _grabbedObject = comp;
                comp.PickUp(_handAnchor);
                OnGrabbedObjectChanged?.Invoke(comp.GetSharedData());
                _status = HandStatus.GRABBED;
            }
        }

        public void OnRMBPressed()
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

        public void OnRMBReleased()
        {
            if(_status == HandStatus.CHARGE && _grabbedObject != null)
            {
                Throw();
            }
        }
    }
}
