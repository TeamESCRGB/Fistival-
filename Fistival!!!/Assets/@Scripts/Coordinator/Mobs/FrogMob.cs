using Coordinator.MobActs;
using Coordinator.Movements;
using Coordinator.Skills;
using Coordinator.Victims;
using Data;
using System.Collections.Generic;
using UnityEngine;

namespace Coordinator.Mobs
{
    public class FrogMob : MobCoordinatorBase, IPushable
    {

        protected bool _isActing;
        protected bool _isAggroOn;
        protected float _skillTime;

        protected TongueAct _act;

        [SerializeField]
        protected float _tongueDuration;
        [SerializeField]
        protected float _tongueStayTime;
        [SerializeField]
        protected float _tongueLength;

        TouchDamageSkill[] _skills;

        protected override void OnAwake()
        {
            base.OnAwake();
            _act = GetComponentInChildren<TongueAct>();
            _skills = GetComponentsInChildren<TouchDamageSkill>(true);
        }

        public override void Init(CommonMobData data)
        {
            base.Init(data);
            _isActing = false;
            _isAggroOn = false;
            _skillTime = _skillDelay;
            
            for(int i = 0; i < _skills.Length; i++)
            {
                _skills[i].Init(data.PlayerHitboxLayer, _damage, _stunTime, _knockbackForce);
            }
            GetComponentInChildren<VictimConnector>().SetOriginal(GetComponentInChildren<VictimCoordinator>());
            _act.Init(() => { _isActing = false; }, _animator, _tongueDuration,_tongueStayTime,_tongueLength);
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

        protected override void OnDead()
        {
            base.OnDead();
            for (int i = 0; i < _skills.Length; i++)
            {
                _skills[i].SetAttackState(false);
            }
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