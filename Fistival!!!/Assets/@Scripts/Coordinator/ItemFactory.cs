using System;
using System.Collections.Generic;
using System.Text;

namespace Coordinator
{
    public class ItemFactory
    {
        private Dictionary<int,ItemBase> _items = new Dictionary<int,ItemBase>();

        public ItemFactory()
        {
            _items[-9999] = new Assets._Scripts.TestScripts.TestItem();
        }

        public ItemBase GetItem(int idx)
        {
            if(_items.TryGetValue(idx,out var data))
            {
                return data;
            }
            return null;
        }
    }
}
