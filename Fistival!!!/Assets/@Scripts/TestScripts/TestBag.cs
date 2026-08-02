using DataStructures;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Utils;

namespace Assets._Scripts.TestScripts
{
    internal class TestBag : MonoBehaviour
    {
        public List<int> bag = new List<int>();
        public RandomBag<int> bag2 = new RandomBag<int>();

        public int[] ints = new int[4];

        private void Start()
        {
            bag2.AddPoolItem(1);
            bag2.AddPoolItem(2);
            bag2.AddPoolItem(3);
            bag2.AddPoolItem(4);
        }

        [ContextMenu("shuffle")]
        void f()
        {
            bag.Shuffle();
        }

        [ContextMenu("pick")]
        void g()
        {
            var idx = bag2.Pick() - 1;
            Debug.Log(idx);
            ints[idx]++;
        }
    }
}
