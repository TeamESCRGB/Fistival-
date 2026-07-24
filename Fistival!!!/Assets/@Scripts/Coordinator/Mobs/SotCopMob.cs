using Coordinator.Door;
using Coordinator.MobActs;
using Coordinator.Movements;
using Coordinator.Skills;
using Data;
using Manager;
using System.Collections.Generic;
using UI;
using UnityEngine;

namespace Coordinator.Mobs
{
    public class SotCopMob : MobCoordinatorBase
    {
        private string _prefabKey;
        [SerializeField]
        protected float _jumpPow;

        [SerializeField]
        private int _slashProjectileIdx;
        [SerializeField]
        protected int _slashCnt;

        protected PlatformerMovementCoordinator _move;

        private MobActBase[] _acts = new MobActBase[3];

        [SerializeField]
        private float _jumpForce;

        protected Transform _player;
        protected bool _isActing;
        protected float _skillTime;

        [SerializeField]
        private int _slamDropObjIdx;
        [SerializeField]
        private int _slamDropObjCnt;
        [SerializeField]
        private float _slamDropObjForce;
        [SerializeField]
        private LayerMask _attackLayer;

        [SerializeField]
        private float _dashStopTime;

        private IReadOnlyList<IDoor> _doors;
        private TouchDamageSkill _touchDamage;

        protected override void OnAwake()
        {
            base.OnAwake();
            _touchDamage = GetComponentInChildren<TouchDamageSkill>();
            _move = GetComponentInChildren<PlatformerMovementCoordinator>();
            _acts[0] = GetComponent<ProjectileLaunchAct>();
            _acts[1] = GetComponent<SlamAct>();
            _acts[2] = GetComponent<LengthDashAct>();
        }

        public override void Init(CommonMobData data)
        {
            base.Init(data);
            _prefabKey = data.PrefabKey;
            _isActing = true;
            _skillTime = _skillDelay;
            _player = FindAnyObjectByType<PlayerCoordinator>().transform;
            _touchDamage.Init(data.PlayerHitboxLayer, _damage, _stunTime, _knockbackForce);
            _move.Init(data.Speed, _jumpPow, 1, 1, GetComponent<Rigidbody2D>());
            ((ProjectileLaunchAct)_acts[0]).Init(() => { _isActing = false; }, _animator, _slashProjectileIdx, _slashCnt, data.PlayerHitboxLayer);
            ((SlamAct)_acts[1]).Init(() => { _isActing = false; }, _animator, GetComponent<Rigidbody2D>(), _slamDropObjIdx,_slamDropObjCnt ,_player, _jumpForce, _slamDropObjForce,_groundLayer,_attackLayer);
            ((LengthDashAct)_acts[2]).Init(() => { _isActing = false; }, _animator, _move, GetComponent<Rigidbody2D>(), data.Speed, _dashStopTime);
            FindAnyObjectByType<PlayerHUD>().SetBoss(GetComponentInChildren<HPCoordinator>(), data.HP);

            for(int i = 0; i < _doors.Count; i++)
            {
                _doors[i].Close();
            }
        }

        public void Init(CommonMobData data, IReadOnlyList<IDoor> doors)
        {
            _doors = doors;
            Init(data);
        }

        public void StartSotCopBoss()
        {
            _isActing = false;
        }

        private void Update()
        {
            if (_isActing)
            {
                return;
            }

            if (_skillTime < _skillDelay)
            {
                _skillTime += Time.deltaTime;
                return;
            }

            if (_stunCounter.IsCooldownEnded() == false)
            {
                return;
            }

            _skillTime = 0;
            _isActing = true;

            if(_player.position.x < transform.position.x)
            {
                _move.OnLeftMovementInputEvent(true);
                _move.OnLeftMovementInputEvent(false);
            }
            else
            {
                _move.OnRightMovementInputEvent(true);
                _move.OnRightMovementInputEvent(false);
            }

            _acts[UnityEngine.Random.Range(0, _acts.Length)].Act();

        }

        protected override void OnDead()
        {
            base.OnDead();
            _touchDamage.SetAttackState(false);
            Managers.Instance.StageManager.ClearBoss(_prefabKey);
            for (int i = 0; i < _doors.Count; i++)
            {
                _doors[i].Open();
            }
        }

        public override void AnimatorOnDead()
        {
            Managers.Instance.ResourceManager.Destroy(gameObject, true);
        }

        protected override void OnHPChanged(int old, int now, int delta)
        {
            
        }

        public override void StunFor(float time)
        {

        }

        public override void ReleaseStun()
        {

        }

        public override void OnStunEnd()
        {

        }

        protected override void OnAggroStateChanged(bool isAggroOn, Collider2D player)
        {
            
        }
    }
}
