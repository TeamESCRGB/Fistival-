using Coordinator;
using Manager;
using UnityEngine;

namespace Utils
{
    public static class ProjectileLaunchHelper
    {
        public  static bool LaunchConstantDir(LayerMask attackLayermask,int idx, in Vector3 initialPos, in Vector2 dir)
        {
            if(Managers.Instance.DataManager.ProjectileDataDict.TryGetValue(idx,out var data) == false)
            {
                return false;
            }

            if(string.IsNullOrEmpty(data.ProjectilePrefabName) || string.IsNullOrEmpty(data.Physics2DMaterialName))
            {
                return false;
            }

            var go = Managers.Instance.ResourceManager.Instantiate(data.ProjectilePrefabName, pooling: true);
            var proj = go.GetComponent<ProjectileCoordinator>();
            proj.Init(attackLayermask,data);
            proj.Launch(initialPos,dir);

            return true;
        }

        public static bool LaunchGuidedProjectile(LayerMask attackLayermask, int idx, in Vector3 initialPos, in Vector2 dir, Transform target)
        {
            if (Managers.Instance.DataManager.ProjectileDataDict.TryGetValue(idx, out var data) == false)
            {
                return false;
            }

            if (string.IsNullOrEmpty(data.ProjectilePrefabName) || string.IsNullOrEmpty(data.Physics2DMaterialName))
            {
                return false;
            }

            var go = Managers.Instance.ResourceManager.Instantiate(data.ProjectilePrefabName, pooling: true);
            var proj = go.GetComponent<GuidedProjectileCoordinator>();
            proj.Init(attackLayermask,target, data);
            proj.Launch(initialPos, dir);

            return true;
        }
    }
}
