using Assets._Scripts.Coordinator.MobActs;
using Coordinator.MobActs;
using Coordinator.Movements;
using Coordinator.Skills;
using Data;
using System;
using System.Collections.Generic;
using System.Text;
using Unity.VisualScripting;
using UnityEngine;

namespace Coordinator.Mobs
{
    public class FlyingDIscThrowMob : MobCoordinatorBase, IPushable
    {

        protected bool _isActing;
        protected bool _isAggroOn;
        protected float _skillTime;
        protected FlyingDiscLaunchAct _act;

        [SerializeField]
        private string _objKey;
        [SerializeField]
        private int _objIdx;
        [SerializeField]
        private float _returnDuration;
        [SerializeField]
        private float _movLen;
        [SerializeField]
        private LayerMask _objectizableLayer;

        protected override void OnAwake()
        {
            base.OnAwake();
            _act = GetComponentInChildren<FlyingDiscLaunchAct>();
        }

        public override void Init(CommonMobData data)
        {
            base.Init(data);
            _isActing = false;
            _isAggroOn = false;
            _skillTime = _skillDelay;
            GetComponentInChildren<TouchDamageSkill>().Init(data.PlayerHitboxLayer, _damage, _stunTime, _knockbackForce);
            _act.Init(() => { _isActing = false; }, _animator, _objKey, _objIdx, _damage, _stunTime, _returnDuration,  _movLen * Mathf.Sign(transform.right.x), _objectizableLayer, data.PlayerHitboxLayer);//소환하는것들 인자에 보는 방향 넣어주도록 수정하기
        }

        protected override void OnAggroStateChanged(bool isAggroOn, Collider2D player)
        {
            _isAggroOn = isAggroOn;
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

            if (_stunCounter.IsCooldownEnded() == false || _isAggroOn == false)
            {
                return;
            }

            _skillTime = 0;
            _isActing = true;
            _act.Act();
        }



        public override void StunFor(float time)
        {
            if (time <= 0 || _stunCounter.GetRemainedTime() >= time)
            {
                return;
            }

            if (_stunCounter.IsCooldownEnded())
            {
                _act.StopAct();
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

        public void PushTo(Vector2 force)
        {
            _rb2d.AddForce(force, ForceMode2D.Impulse);
        }
    }
}
