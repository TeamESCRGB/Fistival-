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


        protected override void OnAggroStateChanged(bool isAggro, Collider2D collider)
        {
            if(isAggro)
            {
                _move.FollowTarget(_aggroSpeed,collider.gameObject.transform);
            }
            else
            {
                _move.StopFollow();
            }
        }
    }
}
