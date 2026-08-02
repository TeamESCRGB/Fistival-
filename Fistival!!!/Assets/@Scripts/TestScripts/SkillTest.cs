using Coordinator.Movements;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Assets._Scripts.TestScripts
{
    public class SkillTest : MonoBehaviour
    {

        public List<Transform> tar;
        public PointMovement mov;
        public int idx;
        public float duration;
        public Rigidbody2D rb2d;


        [ContextMenu("init")]
        public void Init()
        {
            idx = 0;
        }

        [ContextMenu("step")]
        void step()
        {
            if(idx>=tar.Count)
            {
                return;
            }
            mov.ReqStartMove(tar[idx], duration, rb2d, (bool a) => { if (a) { step(); } });
            idx++;
        }

        [ContextMenu("stop")]
        void stop()
        {
            mov.StopMove();
        }

    }
}
