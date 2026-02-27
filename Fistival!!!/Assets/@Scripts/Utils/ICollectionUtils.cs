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
    }
}