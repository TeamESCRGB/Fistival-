using Assets._Scripts.Utils;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Utils;

namespace Coordinator.MobActs.PlatformerBoss
{
    public class PlatformerPhase2Howling : MobActBase
    {
        private IReadOnlyList<Transform> _spawnPoints;
        [SerializeField]
        private int _mobIdx;
        [SerializeField]
        private int _fallingObjectIdx;
        [SerializeField]
        private int _objectIdx;
        public void Init(Action onActionEnd, Animator animator, IReadOnlyList<Transform> spawnPoints)
        {
            Init(onActionEnd, animator);
            _spawnPoints = spawnPoints;
        }

        public void PlatformerPhase2HowlingEnd()
        {
            _onActEnd?.Invoke();
        }

        public void PlatformerPhase2HowlingSpawn()
        {
            var pos = _spawnPoints[UnityEngine.Random.Range(0, _spawnPoints.Count)];
            var idx = UnityEngine.Random.Range(0, 3);

            switch (idx)
            {
                case 0://mob
                    MobSpawnHelper.SpawnBasicMob(_mobIdx, pos);
                    break;
                case 1://fallingObj
                    FallingObjectSpawnHelper.SpawnFallingObject(_fallingObjectIdx, pos.position);
                    break;
                case 2://Obj
                    ObjectSpawnHelper.Spawn(_objectIdx, pos);
                    break;
            }
            

        }

        public override void Act()
        {
            _animator.SetTrigger("Phase2HowlingStart");
        }

        public override void StopAct()
        {
            
        }
    }
}
