using Coordinator.FallingObjectCoordinator;
using Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Assets._Scripts.TestScripts
{
    public class TestFallingObj : MonoBehaviour
    {
        public FallingObjectCoordinator o;

        IEnumerator Test()
        {
            FallingObjectData d = new FallingObjectData()
            {
                AnimControllerName = "",
                BreakableLayerMask=524296,
                Damage=1,
                Scale = new Vector2(1,1),
                KnockBackForce=5,
                ObjSpawnForce=20,
                PrefabKey="",
                SpawnableObjects = new int[] { 4,4},
                StunTime=5
            };
            yield return new WaitForSeconds(0.1f);

            o.Init(d);
        }

        [ContextMenu("a")]
        void Start()
        {
            StartCoroutine(Test());
        }
    }
}
