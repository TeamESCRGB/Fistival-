using Coordinator;
using Data;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Utils;

namespace Assets._Scripts.TestScripts
{
    public class Testlu : MonoBehaviour
    {

        public Transform target;

        public Vector2 dir;

        [ContextMenu("l1")]
        public void Launch1()
        {
            ProjectileLaunchHelper.LaunchConstantDir(4096, -1, transform.position, dir);
        }

        [ContextMenu("l2")]
        public void Launch2()
        {
            ProjectileLaunchHelper.LaunchGuidedProjectile(4096, -2, transform.position, dir,target);
        }
    }
}
