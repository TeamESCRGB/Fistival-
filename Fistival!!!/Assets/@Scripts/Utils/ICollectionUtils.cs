using System.Collections;
using System.Collections.Generic;

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
    }
}