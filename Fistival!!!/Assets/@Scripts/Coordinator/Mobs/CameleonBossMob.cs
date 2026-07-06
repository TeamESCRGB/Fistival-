using Coordinator.MobActs;
using Coordinator.Movements;
using Coordinator.Skills;
using Coordinator.Victims;
using Data;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Coordinator.Mobs
{
    public class CameleonBossMob : MobCoordinatorBase
    {

        private MobActBase[] _acts = new MobActBase[3];

        protected Transform _player;
        protected bool _isActing;
        protected float _skillTime;
        protected IReadOnlyList<Transform> _points;
        protected Transform _head;

        [SerializeField]
        protected float _tongueDuration;
        [SerializeField]
        protected float _tongueStayTime;
        [SerializeField]
        protected float _tongueLength;

        protected override void OnAwake()
        {
            base.OnAwake();
            _head = transform.Find("@Head");
            _acts[0] = GetComponent<TongueAct>();
        }

        public override void Init(CommonMobData data)
        {
            base.Init(data);
            var original = GetComponentInChildren<VictimCoordinator>();
            var connectors = GetComponentsInChildren<VictimConnector>();
            connectors[0].SetOriginal(original);
            connectors[1].SetOriginal(original);

            _points = null;
            _isActing = false;
            _skillTime = _skillDelay;
            _player = FindAnyObjectByType<PlayerCoordinator>().transform;
            var comps = GetComponentsInChildren<TouchDamageSkill>(true);
            comps[0].Init(data.PlayerHitboxLayer, _damage, _stunTime, _knockbackForce);
            comps[1].Init(data.PlayerHitboxLayer, _damage, _stunTime, _knockbackForce);
            ((TongueAct)_acts[0]).Init(() => { _isActing = false; }, _animator, _tongueDuration, _tongueStayTime, _tongueLength, _groundLayer);
        }

        public void Init(CommonMobData data, IReadOnlyList<Transform> points)
        {
            Init(data);
            _points = points;
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
            return;
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

            //_acts[UnityEngine.Random.Range(0, _acts.Length)].Act();

        }



        public override void StunFor(float time)
        {
            if (time <= 0 || _stunCounter.GetRemainedTime() >= time)
            {
                return;
            }

            if (_stunCounter.IsCooldownEnded())
            {
                _movLock.LockMovement();
            }

            _stunCounter.SetCooldownTime(time);
            _stunCounter.StartCooldown();
        }

        public override void ReleaseStun()
        {
            if (_stunCounter is null || _stunCounter.IsCooldownEnded())
            {
                return;
            }
            _stunCounter.StopCooldown();
        }

        public override void OnStunEnd()
        {
            _movLock.UnlockMovement();
        }

        protected override void OnAggroStateChanged(bool isAggroOn, Collider2D player)
        {

        }
    }
}
