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
            _items[0] = new Items.Makta500();
            _items[1] = new Items.Human();
            _items[2] = new Items.Chupa();
            _items[3] = new Items.Malpollo();
            _items[4] = new Items.Aissyang();
            _items[5] = new Items.Agalitol();
            _items[6] = new Items.DrYangban();
            _items[7] = new Items.NitrogenCider();
            _items[8] = new Items.Twenty();
            _items[9] = new Items.MaxSoyMilk();
            _items[10] = new Items.LikeTheLast();
            _items[11] = new Items.RedBox();
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
