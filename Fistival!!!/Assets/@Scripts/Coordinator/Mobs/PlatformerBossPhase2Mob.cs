using Coordinator.MobActs;
using Coordinator.MobActs.PlatformerBoss;
using Coordinator.Movements;
using Coordinator.Skills;
using Coordinator.Victims;
using Data;
using Manager;
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
        private BlockWaveCoordinator _wave;
        private string _prefabKey;

        protected override void OnAwake()
        {
            base.OnAwake();
            _acts[0] = GetComponent<PlatformerPhase2Fist>();
            _acts[1] = GetComponent<AttackFieldAct>();
            _acts[2] = GetComponent<PlatformerPhase2Howling>();
        }

        public override void Init(CommonMobData data)
        {
            base.Init(data);
            _prefabKey = data.PrefabKey;
            _isActing = false;
            _skillTime = _skillDelay;
            _player = FindAnyObjectByType<PlayerCoordinator>().transform;
            var touchDamages = GetComponentsInChildren<TouchDamageSkill>();
            for (int i = 0; i < touchDamages.Length; i++)
            {
                touchDamages[i].Init(data.PlayerHitboxLayer, _damage, _stunTime, _knockbackForce);
            }

            var waveTouchDamages = _wave.GetComponentsInChildren<TouchDamageSkill>();

            for(int i = 0; i < waveTouchDamages.Length; i++)
            {
                waveTouchDamages[i].Init(data.PlayerHitboxLayer, _damage, _stunTime, _knockbackForce);
            }


            var victimConnectors = GetComponentsInChildren<VictimConnector>();
            var victim = GetComponentInChildren<VictimCoordinator>();

            for(int i = 0; i < victimConnectors.Length; i++)
            {
                victimConnectors[i].SetOriginal(victim);
            }

            ((PlatformerPhase2Fist)_acts[0]).Init(() => { _isActing = false; }, _animator, _wave, _spawnPoints);
            ((AttackFieldAct)_acts[1]).Init(() => { _isActing = false; }, _animator);
            ((PlatformerPhase2Howling)_acts[2]).Init(() => { _isActing = false; }, _animator, _spawnPoints);
        }

        public void Init(CommonMobData data, IReadOnlyList<Transform> spawnPoints, BlockWaveCoordinator wave)
        {
            _spawnPoints = spawnPoints;
            _wave= wave;

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

            _acts[UnityEngine.Random.Range(0, _acts.Length)].Act();
        }

        protected override void OnDead()
        {
            base.OnDead();
            Managers.Instance.StageManager.ClearBoss(_prefabKey);
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