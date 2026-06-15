using System;
using System.Collections.Generic;
using System.Text;

namespace Coordinator
{
    public class ItemFactory
    {
        private List<ItemBase> _items = new List<ItemBase>();

        public ItemFactory()
        {

        }

        public ItemBase GetItem(int idx)
        {
            if(idx < 0 || idx >= _items.Count)
            {
                return null;
            }
            return _items[idx];
        }
    }
}
