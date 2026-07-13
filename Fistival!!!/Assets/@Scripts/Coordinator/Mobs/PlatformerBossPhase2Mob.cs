using Coordinator.MobActs;
using Coordinator.MobActs.PlatformerBoss;
using Coordinator.Movements;
using Coordinator.Skills;
using Data;
using System.Collections.Generic;
using UnityEngine;

namespace Coordinator.Mobs
{
    public class PlatformerBossPhase2Mob : MobCoordinatorBase, IPushable
    {
        private MobActBase[] _acts = new MobActBase[3];
        private IReadOnlyList<Transform> _spawnPoints;
        private Transform _player;
        private bool _isActing;
        private float _skillTime;


        protected override void OnAwake()
        {
            base.OnAwake();
            //_acts[0];
            _acts[1] = GetComponent<AttackFieldAct>();
            _acts[2] = GetComponent<PlatformerPhase2Howling>();
        }

        public override void Init(CommonMobData data)
        {
            base.Init(data);
            _isActing = false;
            _skillTime = _skillDelay;
            _player = FindAnyObjectByType<PlayerCoordinator>().transform;
            var touchDamages = GetComponentsInChildren<TouchDamageSkill>();
            for (int i = 0; i < touchDamages.Length; i++)
            {
                touchDamages[i].Init(data.PlayerHitboxLayer, _damage, _stunTime, _knockbackForce);
            }
            ((AttackFieldAct)_acts[1]).Init(() => { _isActing = false; }, _animator);
            ((PlatformerPhase2Howling)_acts[2]).Init(() => { _isActing = false; }, _animator, _spawnPoints);
        }

        public void Init(CommonMobData data, IReadOnlyList<Transform> spawnPoints)
        {
            _spawnPoints = spawnPoints;
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

            _skillTime = 0;
            _isActing = true;

            _acts[2].Act();//UnityEngine.Random.Range(0, _acts.Length)
        }

        #region UnUsed
        protected override void OnHPChanged(int old, int now, int delta)
        {

        }

        public override void StunFor(float time)
        {
        }

        public override void ReleaseStun()
        {
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
        #endregion
    }
}