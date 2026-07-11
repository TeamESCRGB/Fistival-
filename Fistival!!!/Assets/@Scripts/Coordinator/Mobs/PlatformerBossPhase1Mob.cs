using Coordinator.Movements;
using Coordinator.Skills;
using Data;
using UnityEngine;

namespace Coordinator.Mobs
{
    public class PlatformerBossPhase1Mob : MobCoordinatorBase, IPushable
    {

        private MobActBase[] _acts = new MobActBase[3];

        protected Transform _player;
        protected bool _isActing;
        protected float _skillTime;

        protected override void OnAwake()
        {
            base.OnAwake();
        }

        public override void Init(CommonMobData data)
        {
            base.Init(data);
            _isActing = false;
            _skillTime = _skillDelay;
            _player = FindAnyObjectByType<PlayerCoordinator>().transform;
            GetComponentInChildren<TouchDamageSkill>().Init(data.PlayerHitboxLayer, _damage, _stunTime, _knockbackForce);
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
            //_isActing = true;


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

        }

        public override void OnStunEnd()
        {

        }

        protected override void OnAggroStateChanged(bool isAggroOn, Collider2D player)
        {

        }
    }
}
