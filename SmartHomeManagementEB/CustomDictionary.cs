using System;
using System.Collections.Generic;

namespace SmartHomeManagementEB
{
    // A hash table built from scratch using separate chaining (each bucket
    // is a linked list of entries). This is the "custom dictionary" the
    // assignment asks for — it does the same job as Dictionary<TKey,TValue>
    // but you can see and explain exactly how it works internally.
    public class CustomDictionary<TKey, TValue>
    {
        private class Entry
        {
            public TKey Key;
            public TValue Value;
            public Entry Next;
        }

        private Entry[] _buckets;
        private int _capacity;
        private int _count;

        public int Count => _count;

        public CustomDictionary(int capacity = 16)
        {
            _capacity = capacity;
            _buckets = new Entry[_capacity];
        }

        private int GetBucketIndex(TKey key)
        {
            int hash = key.GetHashCode();
            // strip the sign bit so we always get a non-negative index
            return (hash & int.MaxValue) % _capacity;
        }

        // Adds a new key/value pair. Throws if the key already exists —
        // use Update() to change an existing entry instead.
        public void Add(TKey key, TValue value)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));
            if (ContainsKey(key))
                throw new ArgumentException($"Key '{key}' already exists.");

            int index = GetBucketIndex(key);
            _buckets[index] = new Entry { Key = key, Value = value, Next = _buckets[index] };
            _count++;

            // keep the average chain length short by growing when the
            // table gets more than 75% full
            if (_count > _capacity * 0.75)
                Resize();
        }

        // Updates the value for an existing key. Returns false if the key
        // wasn't found (doesn't create a new entry).
        public bool Update(TKey key, TValue value)
        {
            int index = GetBucketIndex(key);
            var current = _buckets[index];
            while (current != null)
            {
                if (current.Key.Equals(key))
                {
                    current.Value = value;
                    return true;
                }
                current = current.Next;
            }
            return false;
        }

        // Removes an entry by key, re-linking around it. Returns false if
        // the key wasn't found.
        public bool Remove(TKey key)
        {
            int index = GetBucketIndex(key);
            Entry current = _buckets[index];
            Entry previous = null;

            while (current != null)
            {
                if (current.Key.Equals(key))
                {
                    if (previous == null)
                        _buckets[index] = current.Next;
                    else
                        previous.Next = current.Next;

                    _count--;
                    return true;
                }
                previous = current;
                current = current.Next;
            }
            return false;
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            int index = GetBucketIndex(key);
            var current = _buckets[index];
            while (current != null)
            {
                if (current.Key.Equals(key))
                {
                    value = current.Value;
                    return true;
                }
                current = current.Next;
            }
            value = default;
            return false;
        }

        // Gets a value by key. Throws if not found — prefer TryGetValue
        // when a missing key is an expected possibility rather than a bug.
        public TValue Get(TKey key)
        {
            if (TryGetValue(key, out TValue value))
                return value;
            throw new KeyNotFoundException($"Key '{key}' not found.");
        }

        public bool ContainsKey(TKey key) => TryGetValue(key, out _);

        // Returns every key/value pair currently stored, as a plain
        // collection the UI can bind to or loop over.
        public IEnumerable<KeyValuePair<TKey, TValue>> GetAll()
        {
            var list = new List<KeyValuePair<TKey, TValue>>();
            foreach (var bucket in _buckets)
            {
                var current = bucket;
                while (current != null)
                {
                    list.Add(new KeyValuePair<TKey, TValue>(current.Key, current.Value));
                    current = current.Next;
                }
            }
            return list;
        }

        // Doubles the bucket count and re-inserts every entry. Keeps
        // lookups close to O(1) as the dictionary grows instead of chains
        // getting longer and longer.
        private void Resize()
        {
            var oldBuckets = _buckets;
            _capacity *= 2;
            _buckets = new Entry[_capacity];
            _count = 0;

            foreach (var bucket in oldBuckets)
            {
                var current = bucket;
                while (current != null)
                {
                    Add(current.Key, current.Value);
                    current = current.Next;
                }
            }
        }
    }
}