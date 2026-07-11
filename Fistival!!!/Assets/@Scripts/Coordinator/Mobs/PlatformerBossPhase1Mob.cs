using Coordinator.MobActs.PlatformerBoss;
using Coordinator.Movements;
using Coordinator.Skills;
using Data;
using UnityEngine;

namespace Coordinator.Mobs
{
    public class PlatformerBossPhase1Mob : MobCoordinatorBase, IPushable
    {

        private MobActBase[] _acts = new MobActBase[3];

        private Transform _player;
        private bool _isActing;
        private float _skillTime;

        Vector3 _objSpawnPointMin;
        Vector3 _objSpawnPointMax;

        protected override void OnAwake()
        {
            base.OnAwake();
            _acts[0] = GetComponent<PlatformerPhase1Slam>();
        }

        public override void Init(CommonMobData data)
        {
            base.Init(data);
            _isActing = false;
            _skillTime = _skillDelay;
            _player = FindAnyObjectByType<PlayerCoordinator>().transform;
            GetComponentInChildren<TouchDamageSkill>().Init(data.PlayerHitboxLayer, _damage, _stunTime, _knockbackForce);
            ((PlatformerPhase1Slam)_acts[0]).Init(() => { _isActing = false; }, _animator, _rb2d, _player, _groundLayer, _objSpawnPointMin, _objSpawnPointMax);
        }

        public void Init(CommonMobData data, Vector3 objSpawnPointMin, Vector3 objSpawnPointMax)
        {
            _objSpawnPointMin = objSpawnPointMin;
            _objSpawnPointMax = objSpawnPointMax;
            Init(data);
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


            _acts[UnityEngine.Random.Range(0, 1)].Act();

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
