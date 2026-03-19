using Coordinator;
using Coordinator.Victims;
using System.Collections.Generic;
using UnityEngine;
using Utils;

namespace Manager.Contents
{
    public class AttackManager : MonoBehaviour
    {
        private Queue<(IAttackable target, SkillCoordinatorBase attacker, int calculatedDamage)> _reqQueue = new Queue<(IAttackable target, SkillCoordinatorBase attacker, int calculatedDamage)>(64);
        private bool _isRequested = false;

        public void Init()
        {
            _isRequested = false;
            _reqQueue.Clear();
        }

        public void RequestAttack(IAttackable target, SkillCoordinatorBase attacker, int calculatedDamage)
        {
            _reqQueue.Enqueue((target, attacker, calculatedDamage));
            _isRequested = true;
        }

        public (int reqCnt, bool isRequested) GetQueueStatus()
        {
            return (_reqQueue.Count, _isRequested);
        }

        private void LateUpdate()
        {
            if (_isRequested == false)
            {
                return;
            }
            
            //여기도 뭐 ispaused같은거 넣어야지
            while(_reqQueue.IsEmpty() == false)
            {
                var req = _reqQueue.Dequeue();

                if(req.attacker == null || req.target is null)
                {
                    continue;
                }

                if(req.attacker.CanAttackTarget(req.target))
                {
                    req.attacker.Act(req.target, req.calculatedDamage);
                }
            }

            _isRequested = false;
        }
    }
}