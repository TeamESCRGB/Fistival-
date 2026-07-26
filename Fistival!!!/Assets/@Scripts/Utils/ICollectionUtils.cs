using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

namespace Utils
{
    public static class ICollectionUtils
    {
        public static bool IsEmpty(this ICollection collection)
        {
            if(collection is null)
            {
                return false;
            }

            return collection.Count <= 0;
        }

        public static void DeepCopy<T>(this IList<T> source, IList<T> dest)
        {
            if(dest is null)
            {
                return;
            }

            dest.Clear();

            for(int i = 0; i  < source.Count; i++)
            {
                dest.Add(source[i]);
            }
        }

        public static void Shuffle<T>(this IList<T> source)
        {
            if(source is null)
            {
                return;
            }
            var maxCnt = source.Count;
            for(int i = 0; i < maxCnt; i++)
            {
                var idx = UnityEngine.Random.Range(0, maxCnt);

                var item = source[i];
                source[i] = source[idx];
                source[idx] = item;
            }
        }
    }
}