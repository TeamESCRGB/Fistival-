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
            _items[0] = new Assets._Scripts.TestScripts.TestItem();
            _items[1] = new Assets._Scripts.TestScripts.TestItem();
            _items[2] = new Assets._Scripts.TestScripts.TestItem();
            _items[3] = new Assets._Scripts.TestScripts.TestItem();
            _items[4] = new Assets._Scripts.TestScripts.TestItem();
            _items[5] = new Assets._Scripts.TestScripts.TestItem();
            _items[6] = new Assets._Scripts.TestScripts.TestItem();
            _items[7] = new Assets._Scripts.TestScripts.TestItem();
            _items[8] = new Assets._Scripts.TestScripts.TestItem();
            _items[9] = new Assets._Scripts.TestScripts.TestItem();
            _items[10] = new Assets._Scripts.TestScripts.TestItem();
            _items[11] = new Assets._Scripts.TestScripts.TestItem();
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
