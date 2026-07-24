using Coordinator;
using Coordinator.Victims;
using DataStructure;
using System.Collections.Generic;
using UnityEngine;
using Utils;

namespace Manager.Contents
{
    public class AttackManager : MonoBehaviour
    {
        private PriorityQueue<(IAttackable target, SkillCoordinatorBase attacker, int calculatedDamage, Vector2 knockbackForce, float calculatedStun)> _reqPQueue =
            new PriorityQueue<(IAttackable target, SkillCoordinatorBase attacker, int calculatedDamage, Vector2 knockbackForce, float calculatedStun)>();
        private bool _isRequested = false;

        public void Init()
        {
            _isRequested = false;
            _reqPQueue.Clear();
        }

        public void RequestAttack(IAttackable target, SkillCoordinatorBase attacker, int calculatedDamage, Vector2 knockbackForce, float calculatedStun)
        {
            _reqPQueue.Enqueue((int)target.GetVictimType(), (target, attacker, calculatedDamage, knockbackForce, calculatedDamage));
            _isRequested = true;
        }

        public (int reqCnt, bool isRequested) GetQueueStatus()
        {
            return (_reqPQueue.Count, _isRequested);
        }

        public void Clear()
        {
            _reqPQueue.Clear();
            _isRequested = false;
        }

        private void LateUpdate()
        {
            if (_isRequested == false)
            {
                return;
            }

            if(Managers.Instance.GameManager.IsGamePaused())
            {
                return;
            }
            
            //여기도 뭐 ispaused같은거 넣어야지
            while(_reqPQueue.IsItEmpty() == false)
            {
                (int, (IAttackable target, SkillCoordinatorBase attacker, int calculatedDamage, Vector2 knockbackForce, float calculatedStun)) elem = default;
                if(_reqPQueue.TryDequeue(ref elem) == false)
                {
                    _reqPQueue.Clear();
                    break;
                }

                var req = elem.Item2;

                if(req.attacker == null || req.target is null)
                {
                    continue;
                }

                if(req.attacker.CanAttackTarget(req.target))
                {
                    req.attacker.Act(req.target, req.calculatedDamage, req.knockbackForce, req.calculatedStun);
                }
            }

            _isRequested = false;
        }
    }
}