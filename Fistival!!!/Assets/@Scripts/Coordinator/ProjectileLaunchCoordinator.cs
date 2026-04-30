using Manager;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Utils
{
    public static class ProjectileLaunchHelper

    {
        public bool LaunchConstantDir(int idx)
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
            proj.Init();

            return true;
        }
    }
}
