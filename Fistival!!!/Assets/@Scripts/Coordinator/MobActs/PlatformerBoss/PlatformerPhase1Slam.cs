using Coordinator.Objects;
using Manager;
using System;
using UnityEngine;
using Data;
using Utils;

namespace Coordinator.MobActs.PlatformerBoss
{
    public class PlatformerPhase1Slam : MobActBase
    {
        private float _gravityConstant;

        [SerializeField]
        private int _objIdx;
        private ObjectData _objData;
        private Transform _player;
        private Rigidbody2D _rb2d;
        private bool _canSpawnObj;
        private LayerMask _ground;
        [SerializeField]
        private Vector2 _slamForce;

        [SerializeField]
        private float _slamYOffset;
        [SerializeField]
        private float _slamMoveTime;
        private Vector3 _tarPos;
        private Vector3 _objSpawnPointMin;
        private Vector3 _objSpawnPointMax;

        private float _moveTime;
        private bool _canStay;

        private GameObject _slamAlert;

        private void Awake()
        {
            _slamAlert = transform.Find("@SlamAlert").gameObject;
        }

        private void Start()
        {
            _slamAlert.SetActive(false);
            Managers.Instance.DataManager.ObjectDataDict.TryGetValue(_objIdx, out _objData);
            _gravityConstant = Mathf.Abs(Physics2D.gravity.y);
        }

        public void Init(Action onActionEnd, Animator animator, Rigidbody2D rb2d,Transform player,LayerMask groundLayer, Vector3 objSpawnPointMin, Vector3 objSpawnPointMax)
        {
            base.Init(onActionEnd, animator);
            _canSpawnObj = false;
            _ground = groundLayer;
            _rb2d = rb2d;
            _player = player;
            _canStay = false;
            _slamAlert.SetActive(false);
            _objSpawnPointMin = objSpawnPointMin;
            _objSpawnPointMax= objSpawnPointMax;
        }

        private void FixedUpdate()
        {
            if(_canStay==false)
            {
                return;
            }
            if(_moveTime < _slamMoveTime)
            {
                _moveTime += Time.fixedDeltaTime;
                return;
            }
            _canStay = false;
            PlatformerPhase1SlamStay();
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (_canSpawnObj == false)
            {
                return;
            }

            if (_objData is null || ((1 << collision.collider.gameObject.layer) & _ground) == 0)
            {
                _canSpawnObj = false;
                return;
            }

            var obj = Managers.Instance.ResourceManager.Instantiate(_objData.PrefabKey, null, true, true).GetComponentInChildren<ObjectCoordinator>();
            obj.transform.position = new Vector3(UnityEngine.Random.Range(_objSpawnPointMin.x, _objSpawnPointMax.x), _objSpawnPointMin.y, _objSpawnPointMin.z);
            obj.Init(_objData);
            _canSpawnObj = false;
            SlamEnd();
        }

        private void SlamEnd()
        {
            _onActEnd?.Invoke();
        }

        public void PlatformerPhase1SlamSlam()
        {
            _slamAlert.SetActive(false);
            _rb2d.simulated = true;
            _rb2d.WakeUp();
            _rb2d.AddForce(_slamForce, ForceMode2D.Impulse);
            _canSpawnObj = true;
        }

        private void PlatformerPhase1SlamStay()
        {
            _slamAlert.SetActive(true);
            _rb2d.linearVelocity = Vector2.zero;
            _rb2d.simulated = false;
            _animator.SetTrigger("PlatformerPhase1SlamStart");
        }

        public void PlatformerPhase1SlamJump()
        {
            _moveTime = 0;
            _canStay = true;
            _tarPos = _player.position;
            _tarPos.y += _slamYOffset;
            var pow = MovementUtils.PreciseCalculateThrowPower(_tarPos - transform.position, _slamMoveTime, _rb2d.linearDamping, 0.001f,_gravityConstant, _rb2d.linearVelocity);
            _rb2d.AddForce(pow, ForceMode2D.Impulse);
        }

        public override void Act()
        {
            _slamAlert.SetActive(false);
            _canSpawnObj = false;
            _animator.SetTrigger("PlatformerPhase1SlamReady");
        }

        public override void StopAct()
        {
            //unused
        }
    }
}
