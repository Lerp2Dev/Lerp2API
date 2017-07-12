using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace Lerp2API.Utility
{
    /// <summary>
    /// Class SerializableDictionary.
    /// </summary>
    /// <typeparam name="TKey">The type of the t key.</typeparam>
    /// <typeparam name="TValue">The type of the t value.</typeparam>
    /// <seealso cref="System.Collections.Generic.IDictionary{TKey, TValue}" />
    [Serializable, DebuggerDisplay("Count = {Count}")]
    public class SerializableDictionary<TKey, TValue> : IDictionary<TKey, TValue>
    {
        [SerializeField, HideInInspector]
        private int[] _Buckets;

        [SerializeField, HideInInspector]
        private int[] _HashCodes;

        [SerializeField, HideInInspector]
        private int[] _Next;

        [SerializeField, HideInInspector]
        private int _Count;

        [SerializeField, HideInInspector]
        private int _Version;

        [SerializeField, HideInInspector]
        private int _FreeList;

        [SerializeField, HideInInspector]
        private int _FreeCount;

        [SerializeField, HideInInspector]
        private TKey[] _Keys;

        [SerializeField, HideInInspector]
        private TValue[] _Values;

        private readonly IEqualityComparer<TKey> _Comparer;

        // Mainly for debugging purposes - to get the key-value pairs display
        /// <summary>
        /// Gets as dictionary.
        /// </summary>
        /// <value>As dictionary.</value>
        public Dictionary<TKey, TValue> AsDictionary
        {
            get { return new Dictionary<TKey, TValue>(this); }
        }

        /// <summary>
        /// Gets the count.
        /// </summary>
        /// <value>The count.</value>
        public int Count
        {
            get { return _Count - _FreeCount; }
        }

        /// <summary>
        /// Gets the <see cref="TValue"/> with the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns>TValue.</returns>
        public TValue this[TKey key, TValue defaultValue]
        {
            get
            {
                int index = FindIndex(key);
                if (index >= 0)
                    return _Values[index];
                return defaultValue;
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="TValue"/> with the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>TValue.</returns>
        /// <exception cref="KeyNotFoundException"></exception>
        public TValue this[TKey key]
        {
            get
            {
                int index = FindIndex(key);
                if (index >= 0)
                    return _Values[index];
                throw new KeyNotFoundException(key.ToString());
            }

            set { Insert(key, value, false); }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SerializableDictionary{TKey, TValue}"/> class.
        /// </summary>
        public SerializableDictionary()
            : this(0, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SerializableDictionary{TKey, TValue}"/> class.
        /// </summary>
        /// <param name="capacity">The capacity.</param>
        public SerializableDictionary(int capacity)
            : this(capacity, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SerializableDictionary{TKey, TValue}"/> class.
        /// </summary>
        /// <param name="comparer">The comparer.</param>
        public SerializableDictionary(IEqualityComparer<TKey> comparer)
            : this(0, comparer)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SerializableDictionary{TKey, TValue}"/> class.
        /// </summary>
        /// <param name="capacity">The capacity.</param>
        /// <param name="comparer">The comparer.</param>
        /// <exception cref="ArgumentOutOfRangeException">capacity</exception>
        public SerializableDictionary(int capacity, IEqualityComparer<TKey> comparer)
        {
            if (capacity < 0)
                throw new ArgumentOutOfRangeException("capacity");

            Initialize(capacity);

            _Comparer = (comparer ?? EqualityComparer<TKey>.Default);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SerializableDictionary{TKey, TValue}"/> class.
        /// </summary>
        /// <param name="dictionary">The dictionary.</param>
        public SerializableDictionary(IDictionary<TKey, TValue> dictionary)
            : this(dictionary, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SerializableDictionary{TKey, TValue}"/> class.
        /// </summary>
        /// <param name="dictionary">The dictionary.</param>
        /// <param name="comparer">The comparer.</param>
        /// <exception cref="ArgumentNullException">dictionary</exception>
        public SerializableDictionary(IDictionary<TKey, TValue> dictionary, IEqualityComparer<TKey> comparer)
            : this((dictionary != null) ? dictionary.Count : 0, comparer)
        {
            if (dictionary == null)
                throw new ArgumentNullException("dictionary");

            foreach (KeyValuePair<TKey, TValue> current in dictionary)
                Add(current.Key, current.Value);
        }

        /// <summary>
        /// Determines whether the specified value contains value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns><c>true</c> if the specified value contains value; otherwise, <c>false</c>.</returns>
        public bool ContainsValue(TValue value)
        {
            if (value == null)
            {
                for (int i = 0; i < _Count; i++)
                {
                    if (_HashCodes[i] >= 0 && _Values[i] == null)
                        return true;
                }
            }
            else
            {
                var defaultComparer = EqualityComparer<TValue>.Default;
                for (int i = 0; i < _Count; i++)
                {
                    if (_HashCodes[i] >= 0 && defaultComparer.Equals(_Values[i], value))
                        return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Determines whether the specified key contains key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns><c>true</c> if the specified key contains key; otherwise, <c>false</c>.</returns>
        public bool ContainsKey(TKey key)
        {
            return FindIndex(key) >= 0;
        }

        /// <summary>
        /// Clears this instance.
        /// </summary>
        public void Clear()
        {
            if (_Count <= 0)
                return;

            for (int i = 0; i < _Buckets.Length; i++)
                _Buckets[i] = -1;

            Array.Clear(_Keys, 0, _Count);
            Array.Clear(_Values, 0, _Count);
            Array.Clear(_HashCodes, 0, _Count);
            Array.Clear(_Next, 0, _Count);

            _FreeList = -1;
            _Count = 0;
            _FreeCount = 0;
            _Version++;
        }

        /// <summary>
        /// Adds the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        public void Add(TKey key, TValue value)
        {
            Insert(key, value, true);
        }

        private void Resize(int newSize, bool forceNewHashCodes)
        {
            int[] bucketsCopy = new int[newSize];
            for (int i = 0; i < bucketsCopy.Length; i++)
                bucketsCopy[i] = -1;

            var keysCopy = new TKey[newSize];
            var valuesCopy = new TValue[newSize];
            var hashCodesCopy = new int[newSize];
            var nextCopy = new int[newSize];

            Array.Copy(_Values, 0, valuesCopy, 0, _Count);
            Array.Copy(_Keys, 0, keysCopy, 0, _Count);
            Array.Copy(_HashCodes, 0, hashCodesCopy, 0, _Count);
            Array.Copy(_Next, 0, nextCopy, 0, _Count);

            if (forceNewHashCodes)
            {
                for (int i = 0; i < _Count; i++)
                {
                    if (hashCodesCopy[i] != -1)
                        hashCodesCopy[i] = (_Comparer.GetHashCode(keysCopy[i]) & 2147483647);
                }
            }

            for (int i = 0; i < _Count; i++)
            {
                int index = hashCodesCopy[i] % newSize;
                nextCopy[i] = bucketsCopy[index];
                bucketsCopy[index] = i;
            }

            _Buckets = bucketsCopy;
            _Keys = keysCopy;
            _Values = valuesCopy;
            _HashCodes = hashCodesCopy;
            _Next = nextCopy;
        }

        private void Resize()
        {
            Resize(PrimeHelper.ExpandPrime(_Count), false);
        }

        /// <summary>
        /// Removes the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        /// <exception cref="ArgumentNullException">key</exception>
        public bool Remove(TKey key)
        {
            if (key == null)
                throw new ArgumentNullException("key");

            int hash = _Comparer.GetHashCode(key) & 2147483647;
            int index = hash % _Buckets.Length;
            int num = -1;
            for (int i = _Buckets[index]; i >= 0; i = _Next[i])
            {
                if (_HashCodes[i] == hash && _Comparer.Equals(_Keys[i], key))
                {
                    if (num < 0)
                        _Buckets[index] = _Next[i];
                    else
                        _Next[num] = _Next[i];

                    _HashCodes[i] = -1;
                    _Next[i] = _FreeList;
                    _Keys[i] = default(TKey);
                    _Values[i] = default(TValue);
                    _FreeList = i;
                    _FreeCount++;
                    _Version++;
                    return true;
                }
                num = i;
            }
            return false;
        }

        private void Insert(TKey key, TValue value, bool add)
        {
            if (key == null)
                throw new ArgumentNullException("key");

            if (_Buckets == null)
                Initialize(0);

            int hash = _Comparer.GetHashCode(key) & 2147483647;
            int index = hash % _Buckets.Length;
            int num1 = 0;
            for (int i = _Buckets[index]; i >= 0; i = _Next[i])
            {
                if (_HashCodes[i] == hash && _Comparer.Equals(_Keys[i], key))
                {
                    if (add)
                        throw new ArgumentException("Key already exists: " + key);

                    _Values[i] = value;
                    _Version++;
                    return;
                }
                num1++;
            }
            int num2;
            if (_FreeCount > 0)
            {
                num2 = _FreeList;
                _FreeList = _Next[num2];
                _FreeCount--;
            }
            else
            {
                if (_Count == _Keys.Length)
                {
                    Resize();
                    index = hash % _Buckets.Length;
                }
                num2 = _Count;
                _Count++;
            }
            _HashCodes[num2] = hash;
            _Next[num2] = _Buckets[index];
            _Keys[num2] = key;
            _Values[num2] = value;
            _Buckets[index] = num2;
            _Version++;

            //if (num3 > 100 && HashHelpers.IsWellKnownEqualityComparer(comparer))
            //{
            //    comparer = (IEqualityComparer<TKey>)HashHelpers.GetRandomizedEqualityComparer(comparer);
            //    Resize(entries.Length, true);
            //}
        }

        private void Initialize(int capacity)
        {
            int prime = PrimeHelper.GetPrime(capacity);

            _Buckets = new int[prime];
            for (int i = 0; i < _Buckets.Length; i++)
                _Buckets[i] = -1;

            _Keys = new TKey[prime];
            _Values = new TValue[prime];
            _HashCodes = new int[prime];
            _Next = new int[prime];

            _FreeList = -1;
        }

        private int FindIndex(TKey key)
        {
            if (key == null)
                throw new ArgumentNullException("key");

            if (_Buckets != null)
            {
                int hash = _Comparer.GetHashCode(key) & 2147483647;
                for (int i = _Buckets[hash % _Buckets.Length]; i >= 0; i = _Next[i])
                {
                    if (_HashCodes[i] == hash && _Comparer.Equals(_Keys[i], key))
                        return i;
                }
            }
            return -1;
        }

        /// <summary>
        /// Tries the get value.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public bool TryGetValue(TKey key, out TValue value)
        {
            int index = FindIndex(key);
            if (index >= 0)
            {
                value = _Values[index];
                return true;
            }
            value = default(TValue);
            return false;
        }

        private static class PrimeHelper
        {
            /// <summary>
            /// The primes
            /// </summary>
            public static readonly int[] Primes = new int[]
            {
            3,
            7,
            11,
            17,
            23,
            29,
            37,
            47,
            59,
            71,
            89,
            107,
            131,
            163,
            197,
            239,
            293,
            353,
            431,
            521,
            631,
            761,
            919,
            1103,
            1327,
            1597,
            1931,
            2333,
            2801,
            3371,
            4049,
            4861,
            5839,
            7013,
            8419,
            10103,
            12143,
            14591,
            17519,
            21023,
            25229,
            30293,
            36353,
            43627,
            52361,
            62851,
            75431,
            90523,
            108631,
            130363,
            156437,
            187751,
            225307,
            270371,
            324449,
            389357,
            467237,
            560689,
            672827,
            807403,
            968897,
            1162687,
            1395263,
            1674319,
            2009191,
            2411033,
            2893249,
            3471899,
            4166287,
            4999559,
            5999471,
            7199369
            };

            /// <summary>
            /// Determines whether the specified candidate is prime.
            /// </summary>
            /// <param name="candidate">The candidate.</param>
            /// <returns><c>true</c> if the specified candidate is prime; otherwise, <c>false</c>.</returns>
            public static bool IsPrime(int candidate)
            {
                if ((candidate & 1) != 0)
                {
                    int num = (int)Math.Sqrt((double)candidate);
                    for (int i = 3; i <= num; i += 2)
                    {
                        if (candidate % i == 0)
                        {
                            return false;
                        }
                    }
                    return true;
                }
                return candidate == 2;
            }

            /// <summary>
            /// Gets the prime.
            /// </summary>
            /// <param name="min">The minimum.</param>
            /// <returns>System.Int32.</returns>
            /// <exception cref="ArgumentException">min < 0</exception>
            public static int GetPrime(int min)
            {
                if (min < 0)
                    throw new ArgumentException("min < 0");

                for (int i = 0; i < PrimeHelper.Primes.Length; i++)
                {
                    int prime = PrimeHelper.Primes[i];
                    if (prime >= min)
                        return prime;
                }
                for (int i = min | 1; i < 2147483647; i += 2)
                {
                    if (PrimeHelper.IsPrime(i) && (i - 1) % 101 != 0)
                        return i;
                }
                return min;
            }

            /// <summary>
            /// Expands the prime.
            /// </summary>
            /// <param name="oldSize">The old size.</param>
            /// <returns>System.Int32.</returns>
            public static int ExpandPrime(int oldSize)
            {
                int num = 2 * oldSize;
                if (num > 2146435069 && 2146435069 > oldSize)
                {
                    return 2146435069;
                }
                return PrimeHelper.GetPrime(num);
            }
        }

        /// <summary>
        /// Gets the keys.
        /// </summary>
        /// <value>The keys.</value>
        public ICollection<TKey> Keys
        {
            get { return _Keys.Take(Count).ToArray(); }
        }

        /// <summary>
        /// Gets the values.
        /// </summary>
        /// <value>The values.</value>
        public ICollection<TValue> Values
        {
            get { return _Values.Take(Count).ToArray(); }
        }

        /// <summary>
        /// Adds the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        public void Add(KeyValuePair<TKey, TValue> item)
        {
            Add(item.Key, item.Value);
        }

        /// <summary>
        /// Determines whether [contains] [the specified item].
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns><c>true</c> if [contains] [the specified item]; otherwise, <c>false</c>.</returns>
        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            int index = FindIndex(item.Key);
            return index >= 0 &&
                EqualityComparer<TValue>.Default.Equals(_Values[index], item.Value);
        }

        /// <summary>
        /// Copies to.
        /// </summary>
        /// <param name="array">The array.</param>
        /// <param name="index">The index.</param>
        /// <exception cref="ArgumentNullException">array</exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int index)
        {
            if (array == null)
                throw new ArgumentNullException("array");

            if (index < 0 || index > array.Length)
                throw new ArgumentOutOfRangeException(string.Format("index = {0} array.Length = {1}", index, array.Length));

            if (array.Length - index < Count)
                throw new ArgumentException(string.Format("The number of elements in the dictionary ({0}) is greater than the available space from index to the end of the destination array {1}.", Count, array.Length));

            for (int i = 0; i < _Count; i++)
            {
                if (_HashCodes[i] >= 0)
                    array[index++] = new KeyValuePair<TKey, TValue>(_Keys[i], _Values[i]);
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is read only.
        /// </summary>
        /// <value><c>true</c> if this instance is read only; otherwise, <c>false</c>.</value>
        public bool IsReadOnly
        {
            get { return false; }
        }

        /// <summary>
        /// Removes the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            return Remove(item.Key);
        }

        /// <summary>
        /// Gets the enumerator.
        /// </summary>
        /// <returns>Enumerator.</returns>
        public Enumerator GetEnumerator()
        {
            return new Enumerator(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Struct Enumerator
        /// </summary>
        /// <seealso cref="System.Collections.Generic.IEnumerator{System.Collections.Generic.KeyValuePair{TKey, TValue}}" />
        public struct Enumerator : IEnumerator<KeyValuePair<TKey, TValue>>
        {
            private readonly SerializableDictionary<TKey, TValue> _Dictionary;
            private int _Version;
            private int _Index;
            private KeyValuePair<TKey, TValue> _Current;

            /// <summary>
            /// Gets the current.
            /// </summary>
            /// <value>The current.</value>
            public KeyValuePair<TKey, TValue> Current
            {
                get { return _Current; }
            }

            internal Enumerator(SerializableDictionary<TKey, TValue> dictionary)
            {
                _Dictionary = dictionary;
                _Version = dictionary._Version;
                _Current = default(KeyValuePair<TKey, TValue>);
                _Index = 0;
            }

            /// <summary>
            /// Moves the next.
            /// </summary>
            /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
            /// <exception cref="InvalidOperationException"></exception>
            public bool MoveNext()
            {
                if (_Version != _Dictionary._Version)
                    throw new InvalidOperationException(string.Format("Enumerator version {0} != Dictionary version {1}", _Version, _Dictionary._Version));

                while (_Index < _Dictionary._Count)
                {
                    if (_Dictionary._HashCodes[_Index] >= 0)
                    {
                        _Current = new KeyValuePair<TKey, TValue>(_Dictionary._Keys[_Index], _Dictionary._Values[_Index]);
                        _Index++;
                        return true;
                    }
                    _Index++;
                }

                _Index = _Dictionary._Count + 1;
                _Current = default(KeyValuePair<TKey, TValue>);
                return false;
            }

            void IEnumerator.Reset()
            {
                if (_Version != _Dictionary._Version)
                    throw new InvalidOperationException(string.Format("Enumerator version {0} != Dictionary version {1}", _Version, _Dictionary._Version));

                _Index = 0;
                _Current = default(KeyValuePair<TKey, TValue>);
            }

            object IEnumerator.Current
            {
                get { return Current; }
            }

            /// <summary>
            /// Disposes this instance.
            /// </summary>
            public void Dispose()
            {
            }
        }
    }
}