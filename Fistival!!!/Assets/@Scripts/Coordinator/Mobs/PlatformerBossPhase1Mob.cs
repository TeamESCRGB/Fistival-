using Coordinator.MobActs;
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

        private int _hpHalf;
        private bool _hpHalfPatternFlag;
        private bool _hpHalfPatternExecutedFlag;


        protected override void OnAwake()
        {
            base.OnAwake();
            _acts[0] = GetComponent<PlatformerPhase1Slam>();
            _acts[1] = GetComponent<AttackFieldAct>();
        }

        public override void Init(CommonMobData data)
        {
            base.Init(data);
            _hpHalf = data.HP / 2;
            _isActing = false;
            _skillTime = _skillDelay;
            _player = FindAnyObjectByType<PlayerCoordinator>().transform;
            _hpHalfPatternFlag = false;
            _hpHalfPatternExecutedFlag = false;
            var touchDamages = GetComponentsInChildren<TouchDamageSkill>();
            for(int i = 0; i < touchDamages.Length; i++)
            {
                touchDamages[i].Init(data.PlayerHitboxLayer, _damage, _stunTime, _knockbackForce);
            }
            ((PlatformerPhase1Slam)_acts[0]).Init(() => { _isActing = false; }, _animator, _rb2d, _player, _groundLayer, _objSpawnPointMin, _objSpawnPointMax);
            ((AttackFieldAct)_acts[1]).Init(() => { _isActing = false; }, _animator);
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

            var rot = transform.eulerAngles;

            if(_player.position.x < transform.position.x)
            {
                rot.z = 180;
            }
            else
            {
                rot.z = 0;
            }

            transform.eulerAngles = rot;

            _skillTime = 0;
            _isActing = true;

            if(_hpHalfPatternFlag)
            {
                Debug.Log("asdf패턴");
                _hpHalfPatternFlag = false;
            }
            else
            {
                _acts[UnityEngine.Random.Range(1, 2)].Act();
            }
        }

        protected override void OnHPChanged(int old, int now, int delta)
        {
            base.OnHPChanged(old, now, delta);
            if(_hpHalfPatternExecutedFlag)
            {
                return;
            }
            if(now <= _hpHalf)
            {
                _hpHalfPatternFlag = true;
                _hpHalfPatternExecutedFlag = true;
            }
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
    }
}
