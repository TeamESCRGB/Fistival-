using System;
using System.Collections.Generic;
using System.Text;
using Utils;

namespace DataStructures
{
    public class RandomBag<T>
    {
        private List<int> _bag = new List<int>(4);
        private List<T> _items = new List<T>(4);
        public void ClearBag()
        {
            _bag.Clear();
        }
        public void Init()
        {
            _bag.Clear();
            _items.Clear();
        }
        public void RefillBag()
        {
            _bag.Clear();
            for (int i = 0; i < _items.Count; i++)
            {
                _bag.Add(i);
            }

            _bag.Shuffle();
        }

        public void AddPoolItem(T item)
        {
            _items.Add(item);
        }

        public T Pick()
        {
            var count = _bag.Count;
            if (count <= 0)
            {
                RefillBag();
            }
            count = _bag.Count;

            var item = _items[_bag[count-1]];
            _bag.RemoveAt(count - 1);
            return item;
        }
    }
}
