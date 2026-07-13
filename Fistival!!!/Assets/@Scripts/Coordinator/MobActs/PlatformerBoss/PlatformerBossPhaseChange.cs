using Coordinator.Mobs;
using Manager;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Utils;

namespace Coordinator.MobActs.PlatformerBoss
{
    public class PlatformerBossPhaseChange : MobActBase
    {
        [SerializeField]
        private int _phase2Idx;
        [SerializeField]
        private float _phaseChangeJumpDuration;
        private Transform _endPos;
        private Rigidbody2D _rb2d;
        private float _fdt;
        private bool _isMoving;
        private IReadOnlyList<Transform> _spawnPoints;
        private BlockWaveCoordinator _blockWave;
        public void Init(Action onActionEnd, Animator animator, Transform endPos, Rigidbody2D rb2d, IReadOnlyList<Transform> phase2ObjSpawnPoints, BlockWaveCoordinator phase2WaveCoord)
        {
            Init(onActionEnd, animator);
            _endPos= endPos;
            _rb2d= rb2d;
            _spawnPoints= phase2ObjSpawnPoints;
            _blockWave= phase2WaveCoord;
        }

        private void FixedUpdate()
        {
            if(_isMoving == false)
            {
                return;
            }

            _fdt += Time.fixedDeltaTime;
            if(_fdt < _phaseChangeJumpDuration)
            {
                return;
            }

            _isMoving = false;

            var data = Managers.Instance.DataManager.CommonMobDataDict[_phase2Idx];
            var go = Managers.Instance.ResourceManager.Instantiate(data.PrefabKey, null, true, true);
            var comp = go.GetComponent<PlatformerBossPhase2Mob>();
            comp.Init(data, _spawnPoints, _blockWave);
            go.transform.position = _endPos.position;
            _onActEnd?.Invoke();
        }

        public void ChangePlatformerBossPhase()
        {
            var force = MovementUtils.PreciseCalculateThrowPower(_endPos.position - transform.position, _phaseChangeJumpDuration, _rb2d.linearDamping, 0.001f, Mathf.Abs(Physics2D.gravity.y), _rb2d.linearVelocity);
            _rb2d.AddForce(force, ForceMode2D.Impulse);
            _isMoving = true;
        }

        public override void Act()
        {
            _isMoving = false;
            _fdt = 0;
            _animator.SetTrigger("ChangePhase");
        }


        public override void StopAct()
        {
            
        }
    }
}
