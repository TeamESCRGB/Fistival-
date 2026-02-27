using System.Collections.Generic;

namespace Data.DataLoaders
{
    public interface ILoader<TKey, TVal>
    {
        Dictionary<TKey, TVal> MakeDict();
    }
}