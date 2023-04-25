using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gameframe.Procgen
{
    public interface IReadOnlyWeightedItemCollection<TItem> : IEnumerable<(TItem, int)>
    {
        int GetWeight(TItem item);
        int TotalWeight { get; }
    }

    public interface IWeightedItemCollection<TItem> : IReadOnlyWeightedItemCollection<TItem>
    {
        void Add(TItem item, int weight);
        void Remove(TItem item);
        void Clear();
    }

    public abstract class WeightedItemCollection<TItem> : IWeightedItemCollection<TItem>
    {
        private Dictionary<TItem, int> _dictionary = new Dictionary<TItem, int>();

        public int GetWeight(TItem item)
        {
            return _dictionary[item];
        }

        public abstract int TotalWeight { get; }

        public abstract void Add(TItem item, int weight);

        public abstract void Remove(TItem item);

        public abstract void Clear();

        public IEnumerator<(TItem, int)> GetEnumerator()
        {
            foreach (var pair in _dictionary)
            {
                yield return (pair.Key, pair.Value);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
