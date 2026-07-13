using Coordinator.MobActs;
using Coordinator.MobActs.CameleonBossPattern;
using Coordinator.Movements;
using Coordinator.Skills;
using Coordinator.Victims;
using Data;
using Defines;
using Manager;
using MobActs.CameleonBossPattern;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Utils;

namespace Coordinator.Mobs
{
    public class CameleonBossMob : MobCoordinatorBase, IPushable
    {

        private MobActBase[] _acts = new MobActBase[3];
        
        protected Transform _player;
        protected bool _isActing;
        protected float _skillTime;
        protected Transform _head;

        [SerializeField]
        protected float _tongueDuration;
        [SerializeField]
        protected float _tongueStayTime;
        [SerializeField]
        protected float _tongueLength;

        [SerializeField]
        private float _moveInterval;
        [SerializeField]
        protected float _moveDuration;
        [SerializeField]
        protected int _objIdx;
        protected ObjectData _objData;
        protected Transform _objSpawnPoint;

        private Vector2 _centerPos;
        private IReadOnlyList<Transform> _points;

        private int _nowAct;
        private VictimCoordinator _victim;
        private PointMovement _mov;

        protected override void OnAwake()
        {
            base.OnAwake();
            _victim = GetComponentInChildren<VictimCoordinator>();
            _nowAct = 0;
            //_head = transform.Find("@Head");
            _head = gameObject.GetChildGameObject("@Head",true).transform;
            _acts[0] = GetComponent<TongueAct>();
            _acts[1] = GetComponent<CameleonPattern2>();
            _acts[2] = GetComponent<CameleonPattern3>();
            _objData = Managers.Instance.DataManager.ObjectDataDict[_objIdx];
            _mov = GetComponent<PointMovement>();
        }

        public override void Init(CommonMobData data)
        {
            base.Init(data);
            var original = GetComponentInChildren<VictimCoordinator>();
            var connectors = GetComponentsInChildren<VictimConnector>();
            connectors[0].SetOriginal(original);
            connectors[1].SetOriginal(original);
            _isActing = false;
            _skillTime = _skillDelay;
            _player = FindAnyObjectByType<PlayerCoordinator>().transform;
            var comps = GetComponentsInChildren<TouchDamageSkill>(true);
            comps[0].Init(data.PlayerHitboxLayer, _damage, _stunTime, _knockbackForce);
            comps[1].Init(data.PlayerHitboxLayer, _damage, _stunTime, _knockbackForce);
            ((TongueAct)_acts[0]).Init(() => { _isActing = false; }, _animator, _tongueDuration, _tongueStayTime, _tongueLength, _groundLayer);
            ((CameleonPattern2)_acts[1]).Init(() => { _isActing = false; }, _animator, _rb2d, _mov, _moveInterval, _moveDuration, _points, _centerPos);
            ((CameleonPattern3)_acts[2]).Init(() => { _isActing = false; }, _animator, _rb2d, _mov, _moveDuration, _points, _centerPos, _objData, _objSpawnPoint);
            _victim.SetAttackableState(false);
        }

        public void Init(CommonMobData data, Vector2 centerPos, IReadOnlyList<Transform> movPoints, Transform objSpawnPoint)
        {
            _points = movPoints;
            _centerPos = centerPos;
            _objSpawnPoint=objSpawnPoint;
            Init(data);
        }

        private void Update()
        {
            
            if (_isActing)
            {
                return;
            }
            float dirSign = Mathf.Sign(transform.right.x);
            Vector3 direction = -(_player.position - _head.position) * dirSign;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg * dirSign;
            Vector3 localeular = _head.localEulerAngles;
            _head.localRotation = Quaternion.Euler(localeular.x, localeular.y, angle);
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

            _nowAct = UnityEngine.Random.Range(0, _acts.Length);
            _acts[_nowAct].Act();//

        }


        public void PaintStun(float time)
        {
            if (time <= 0 || _stunCounter.GetRemainedTime() >= time)
            {
                return;
            }

            if (_stunCounter.IsCooldownEnded())
            {
                _victim.SetAttackableState(true);
                _acts[_nowAct].StopAct();
                _rb2d.gravityScale = 1;
                var rot = transform.eulerAngles;
                rot.z = 90;
                transform.eulerAngles = rot;
                Vector3 localeular = _head.localEulerAngles;
                _head.localRotation = Quaternion.Euler(localeular.x, localeular.y, 0);
                _isActing = true;
            }
            _skillTime = 0;
            _animator.SetTrigger("Stun");
            _stunCounter.SetCooldownTime(time);
            _stunCounter.StartCooldown();
        }

        public override void OnStunEnd()
        {
            _rb2d.gravityScale = 0;
            _victim.SetAttackableState(false);

            if (transform.position.x > _centerPos.x)
            {
                var rot = _rb2d.transform.eulerAngles;
                rot.z = 0;
                rot.y = 0;
                _rb2d.transform.eulerAngles = rot;
                _mov.ReqStartMove(_points[_points.Count - 1], _moveDuration, _rb2d, (bool result) => { _isActing = false; });
            }
            else
            {
                var rot = _rb2d.transform.eulerAngles;
                rot.z = 0;
                rot.y = 180;
                _rb2d.transform.eulerAngles = rot;
                _mov.ReqStartMove(_points[0], _moveDuration, _rb2d, (bool result) => { _isActing = false; });
            }

        }

        public override void StunFor(float time)
        {

        }


        public override void ReleaseStun()
        {
            if (_stunCounter is null || _stunCounter.IsCooldownEnded())
            {
                return;
            }
            _rb2d.gravityScale = 0;
            _stunCounter.StopCooldown();
        }

        protected override void OnAggroStateChanged(bool isAggroOn, Collider2D player)
        {

        }

        protected override void OnHPChanged(int old, int now, int delta)
        {
            
        }

        public void PushTo(Vector2 force)
        {
            _rb2d.AddForce(force, ForceMode2D.Impulse);
        }
    }
}
