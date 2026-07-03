using Assets._Scripts.Coordinator.MobActs;
using UnityEngine;
using Utils;

namespace Coordinator.MobActs
{
    partial class ProjectileLaunchAct : GravityProjectileLaunchAct
    {
        private Transform _launchPos;

        private void Awake()
        {
            _launchPos = transform.Find("@ProjectileLaunchAnchor");
        }

        public override void Launch()
        {
            if (_shootCnt <= 0)
            {
                return;
            }
            _shootCnt--;
            ProjectileLaunchHelper.LaunchConstantDir(_target, _projectileIdx, _launchPos.position, transform.right);
            _animator.SetInteger("ShootCnt", _shootCnt);
        }
    }
}