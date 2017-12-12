using System;
using System.Collections.Generic;

namespace Client
{
    public class OrderPersistence
    {
        private static int _counter = 1;

        public readonly Dictionary<int, Guid> OrderDictionary;

        public OrderPersistence()
        {
            OrderDictionary = new Dictionary<int, Guid>();
        }

        public void Add(Guid id)
        {
            OrderDictionary.Add(_counter, id);
            _counter++;
        }
    }
}