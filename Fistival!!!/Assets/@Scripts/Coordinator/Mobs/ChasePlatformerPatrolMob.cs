using Data;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Coordinator.Mobs
{
    public class ChasePlatformerPatrolMob : PlatformerPatrolMob
    {
        [SerializeField]
        private float _aggroSpeed;

        public override void Init(CommonMobData data)
        {
            base.Init(data);
            _animator.SetBool("Chasing", false);
        }

        protected override void OnAggroStateChanged(bool isAggro, Collider2D collider)
        {
            if(isAggro)
            {
                _animator.SetBool("Chasing", true);
                _move.FollowTarget(_aggroSpeed,collider.gameObject.transform);
            }
            else
            {
                _animator.SetBool("Chasing", false);
                _move.StopFollow();
            }
        }
    }
}
