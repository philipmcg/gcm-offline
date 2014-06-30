using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Utilities
{

    public interface IProvider<K, V>
    {
        bool ContainsKey(K key);
        V this[K key] { get; }
    }

    public class Provider<V> : IProvider<int, V> where V : struct, IEquatable<V>
    {
        public readonly V[] Array;
        V defaultValue;
        Func<int, V> getValue;

        public Provider(int capacity, Func<int, V> getValue, V defaultValue)
        {
            Array = new V[capacity];
            this.defaultValue = defaultValue;
            this.getValue = getValue;
        }

        public bool ContainsKey(int key)
        {
            if (key >= Array.Length)
                return false;

            return !Array[key].Equals(defaultValue);
        }

        public V this[int key]
        {
            get
            {
                if (key >= Array.Length)
                    return getValue(key);

                if (Array[key].Equals(defaultValue))
                    Array[key] = getValue(key);
                return Array[key];
            }
        }
    }

    public class ProviderDictionary<V> : IProvider<int, V> where V : struct, IEquatable<V>
    {
        public readonly System.Collections.Concurrent.ConcurrentDictionary<int,V> Array;
        Func<int, V> getValue;

        public ProviderDictionary(Func<int, V> getValue)
        {
            Array = new System.Collections.Concurrent.ConcurrentDictionary<int, V>();
            this.getValue = getValue;
        }

        public bool ContainsKey(int key)
        {
            return Array.ContainsKey(key);
        }

        public V this[int key]
        {
            get
            {
                if (!Array.ContainsKey(key))
                {
                    var value = getValue(key);
                    Array[key] = value;
                    return value;
                }
                return Array[key];
            }
        }
    }
}
