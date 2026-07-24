using Coordinator.Door;
using Coordinator.MobActs;
using Coordinator.MobActs.PlatformerBoss;
using Coordinator.Movements;
using Coordinator.Skills;
using Data;
using Manager;
using System.Collections.Generic;
using UI;
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
        [SerializeField]
        private List<int> _fallingObjectIdx;
        [SerializeField]
        private float _fallingObjectInterval;

        private Transform _phase2SpawnPoint;
        private PlatformerBossPhaseChange _phaseChangeAct;
        private IReadOnlyList<Transform> _phase2ObjSpawnPoints;
        private BlockWaveCoordinator _phase2WaveCoord;
        private GameObject _phase2Platform;
        private IReadOnlyList<IDoor> _doors;
        protected TouchDamageSkill[] _touchDamages;
        protected MobActBase _nowAct;
        protected override void OnAwake()
        {
            base.OnAwake();
            _touchDamages = GetComponentsInChildren<TouchDamageSkill>();
            _phaseChangeAct = GetComponent<PlatformerBossPhaseChange>();
            _acts[0] = GetComponent<PlatformerPhase1Slam>();
            _acts[1] = GetComponent<AttackFieldAct>();
            _acts[2] = GetComponent<FallingObjectRandomPosSpawnAct>();

        }

        public override void Init(CommonMobData data)
        {
            base.Init(data);
            _hpHalf = data.HP / 2;
            _isActing = true;
            _skillTime = _skillDelay;
            _player = FindAnyObjectByType<PlayerCoordinator>().transform;
            _hpHalfPatternFlag = false;
            _hpHalfPatternExecutedFlag = false;
            for(int i = 0; i < _touchDamages.Length; i++)
            {
                _touchDamages[i].Init(data.PlayerHitboxLayer, _damage, _stunTime, _knockbackForce);
            }
            ((PlatformerPhase1Slam)_acts[0]).Init(() => { _isActing = false; }, _animator, _rb2d, _player, _groundLayer, _objSpawnPointMin, _objSpawnPointMax);
            ((AttackFieldAct)_acts[1]).Init(() => { _isActing = false; }, _animator);
            ((FallingObjectRandomPosSpawnAct)_acts[2]).Init(() => { _isActing = false; }, _animator, _fallingObjectIdx, _objSpawnPointMin, _objSpawnPointMax, _fallingObjectInterval);

            _phaseChangeAct.Init(OnPhaseChanged,_animator, _phase2SpawnPoint, _rb2d, _phase2ObjSpawnPoints, _phase2WaveCoord, _phase2Platform, _doors);

            FindAnyObjectByType<PlayerHUD>().SetBoss(GetComponentInChildren<HPCoordinator>(), data.HP);

            for (int i = 0; i < _doors.Count; i++)
            {
                _doors[i].Close();
            }
        }

        public void StartPlatformerPhase1()
        {
            _isActing = false;
        }

        private void OnPhaseChanged()
        {
            Managers.Instance.ResourceManager.Destroy(gameObject, true);
        }

        public void Init(CommonMobData data, Vector3 objSpawnPointMin, Vector3 objSpawnPointMax, Transform phase2Pos, IReadOnlyList<Transform> phase2ObjSpawnPoints, BlockWaveCoordinator phase2WaveCoord, GameObject phase2Platform, IReadOnlyList<IDoor> doors)
        {
            _doors = doors;
            _phase2WaveCoord = phase2WaveCoord;
            _phase2ObjSpawnPoints = phase2ObjSpawnPoints;
            _phase2SpawnPoint= phase2Pos;
            _objSpawnPointMin = objSpawnPointMin;
            _objSpawnPointMax = objSpawnPointMax;
            _phase2Platform = phase2Platform;
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
                _acts[2].Act();
                _hpHalfPatternFlag = false;
            }
            else
            {
                _nowAct = _acts[UnityEngine.Random.Range(0, 2)];
                _nowAct.Act();
            }
        }
        protected override void OnDead()
        {
            base.OnDead();
            _nowAct.StopAct();
            for (int i = 0; i < _touchDamages.Length; i++)
            {
                _touchDamages[i].SetAttackState(false);
            }
        }
        public override void AnimatorOnDead()
        {
            _phaseChangeAct.Act();
        }

        protected override void OnHPChanged(int old, int now, int delta)
        {
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
