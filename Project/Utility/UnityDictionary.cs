//  Copyright (c) 2012 Calvin Rien
//        http://the.darktable.com
//
// This software is provided 'as-is', without any express or implied warranty. In
// no event will the authors be held liable for any damages arising from the use
// of this software.
//
// Permission is granted to anyone to use this software for any purpose,
// including commercial applications, and to alter it and redistribute it freely,
// subject to the following restrictions:
//
// 1. The origin of this software must not be misrepresented; you must not claim
// that you wrote the original software. If you use this software in a product,
// an acknowledgment in the product documentation would be appreciated but is not
// required.
//
// 2. Altered source versions must be plainly marked as such, and must not be
// misrepresented as being the original software.
//
// 3. This notice may not be removed or altered from any source distribution.

using System.Collections;
using System.Collections.Generic;

namespace Lerp2API.Utility
{
    /// <summary>
    /// Class UnityNameValuePair.
    /// </summary>
    /// <typeparam name="V"></typeparam>
    /// <seealso cref="Lerp2API.Utility.UnityKeyValuePair{System.String, V}" />
    public class UnityNameValuePair<V> : UnityKeyValuePair<string, V>
    {
        /// <summary>
        /// The name
        /// </summary>
        public string name = null;

        /// <summary>
        /// Gets or sets the key.
        /// </summary>
        /// <value>The key.</value>
        override public string Key
        {
            get { return name; }
            set { name = value; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UnityNameValuePair{V}"/> class.
        /// </summary>
        public UnityNameValuePair() : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UnityNameValuePair{V}"/> class.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        public UnityNameValuePair(string key, V value) : base(key, value)
        {
        }
    }

    /// <summary>
    /// Class UnityKeyValuePair.
    /// </summary>
    /// <typeparam name="K"></typeparam>
    /// <typeparam name="V"></typeparam>
    public class UnityKeyValuePair<K, V>
    {
        /// <summary>
        /// Gets or sets the key.
        /// </summary>
        /// <value>The key.</value>
        virtual public K Key
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>The value.</value>
        virtual public V Value
        {
            get;
            set;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UnityKeyValuePair{K, V}"/> class.
        /// </summary>
        public UnityKeyValuePair()
        {
            Key = default(K);
            Value = default(V);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UnityKeyValuePair{K, V}"/> class.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        public UnityKeyValuePair(K key, V value)
        {
            Key = key;
            Value = value;
        }
    }

    /// <summary>
    /// Class UnityDictionary.
    /// </summary>
    /// <typeparam name="K"></typeparam>
    /// <typeparam name="V"></typeparam>
    /// <seealso cref="System.Collections.Generic.IDictionary{K, V}" />
    public abstract class UnityDictionary<K, V> : IDictionary<K, V>
    {
        /// <summary>
        /// Gets or sets the key value pairs.
        /// </summary>
        /// <value>The key value pairs.</value>
        abstract protected List<UnityKeyValuePair<K, V>> KeyValuePairs
        {
            get;
            set;
        }

        /// <summary>
        /// Sets the key value pair.
        /// </summary>
        /// <param name="k">The k.</param>
        /// <param name="v">The v.</param>
        protected abstract void SetKeyValuePair(K k, V v); /* {

      var index = Collection.FindIndex(x => {return x.Key == k;});

      if (index != -1) {
        if (v == null) {
          Collection.RemoveAt(index);
          return;
        }

        values[index] = new UnityKeyValuePair(key, value);
        return;
      }

      values.Add(new UnityKeyValuePair(key, value));
    } */

        /// <summary>
        /// Gets or sets the <see cref="V"/> with the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>V.</returns>
        virtual public V this[K key]
        {
            get
            {
                var result = KeyValuePairs.Find(x =>
                {
                    return x.Key.Equals(key);
                });

                if (result == null)
                {
                    return default(V);
                }

                return result.Value;
            }
            set
            {
                if (key == null)
                {
                    return;
                }

                SetKeyValuePair(key, value);
            }
        }

        #region IDictionary interface

        /// <summary>
        /// Adds the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        public void Add(K key, V value)
        {
            this[key] = value;
        }

        /// <summary>
        /// Adds the specified KVP.
        /// </summary>
        /// <param name="kvp">The KVP.</param>
        public void Add(KeyValuePair<K, V> kvp)
        {
            this[kvp.Key] = kvp.Value;
        }

        /// <summary>
        /// Tries the get value.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public bool TryGetValue(K key, out V value)
        {
            if (!this.ContainsKey(key))
            {
                value = default(V);
                return false;
            }

            value = this[key];
            return true;
        }

        /// <summary>
        /// Removes the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public bool Remove(KeyValuePair<K, V> item)
        {
            return Remove(item.Key);
        }

        /// <summary>
        /// Removes the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public bool Remove(K key)
        {
            var list = KeyValuePairs;

            var index = list.FindIndex(x =>
            {
                return x.Key.Equals(key);
            });

            if (index == -1)
            {
                return false;
            }

            list.RemoveAt(index);

            KeyValuePairs = list;

            return true;
        }

        /// <summary>
        /// Clears this instance.
        /// </summary>
        public void Clear()
        {
            var list = KeyValuePairs;

            list.Clear();

            KeyValuePairs = list;
        }

        /// <summary>
        /// Determines whether the specified key contains key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns><c>true</c> if the specified key contains key; otherwise, <c>false</c>.</returns>
        public bool ContainsKey(K key)
        {
            return KeyValuePairs.FindIndex(x =>
            {
                return x.Key.Equals(key);
            }) != -1;
        }

        /// <summary>
        /// Determines whether [contains] [the specified KVP].
        /// </summary>
        /// <param name="kvp">The KVP.</param>
        /// <returns><c>true</c> if [contains] [the specified KVP]; otherwise, <c>false</c>.</returns>
        public bool Contains(KeyValuePair<K, V> kvp)
        {
            return this[kvp.Key].Equals(kvp.Value);
        }

        /// <summary>
        /// Gets the count.
        /// </summary>
        /// <value>The count.</value>
        public int Count
        {
            get
            {
                return KeyValuePairs.Count;
            }
        }

        /// <summary>
        /// Copies to.
        /// </summary>
        /// <param name="array">The array.</param>
        /// <param name="index">The index.</param>
        public void CopyTo(KeyValuePair<K, V>[] array, int index)
        {
            var copy = KeyValuePairs.ConvertAll<KeyValuePair<K, V>>(
              new System.Converter<UnityKeyValuePair<K, V>, KeyValuePair<K, V>>(
              x =>
              {
                  return new KeyValuePair<K, V>(x.Key, (V)x.Value);
              }));

            copy.CopyTo(array, index);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator() as IEnumerator;
        }

        /// <summary>
        /// Gets the enumerator.
        /// </summary>
        /// <returns>IEnumerator&lt;KeyValuePair&lt;K, V&gt;&gt;.</returns>
        public IEnumerator<KeyValuePair<K, V>> GetEnumerator()
        {
            return new UnityDictionaryEnumerator(this);
        }

        /// <summary>
        /// Gets the keys.
        /// </summary>
        /// <value>The keys.</value>
        public ICollection<K> Keys
        {
            get
            {
                return KeyValuePairs.ConvertAll(new System.Converter<UnityKeyValuePair<K, V>, K>(x =>
                {
                    return x.Key;
                }));
            }
        }

        /// <summary>
        /// Gets the values.
        /// </summary>
        /// <value>The values.</value>
        public ICollection<V> Values
        {
            get
            {
                return KeyValuePairs.ConvertAll(new System.Converter<UnityKeyValuePair<K, V>, V>(x =>
                {
                    return x.Value;
                }));
            }
        }

        /// <summary>
        /// Gets the items.
        /// </summary>
        /// <value>The items.</value>
        public ICollection<KeyValuePair<K, V>> Items
        {
            get
            {
                return KeyValuePairs.ConvertAll<KeyValuePair<K, V>>(new System.Converter<UnityKeyValuePair<K, V>, KeyValuePair<K, V>>(x =>
                {
                    return new KeyValuePair<K, V>(x.Key, x.Value);
                }));
            }
        }

        /// <summary>
        /// Gets the synchronize root.
        /// </summary>
        /// <value>The synchronize root.</value>
        public V SyncRoot
        {
            get { return default(V); }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is fixed size.
        /// </summary>
        /// <value><c>true</c> if this instance is fixed size; otherwise, <c>false</c>.</value>
        public bool IsFixedSize
        {
            get { return false; }
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
        /// Gets a value indicating whether this instance is synchronized.
        /// </summary>
        /// <value><c>true</c> if this instance is synchronized; otherwise, <c>false</c>.</value>
        public bool IsSynchronized
        {
            get { return false; }
        }

        internal sealed class UnityDictionaryEnumerator : IEnumerator<KeyValuePair<K, V>>
        {
            // A copy of the SimpleDictionary T's key/value pairs.
            private KeyValuePair<K, V>[] items;

            private int index = -1;

            internal UnityDictionaryEnumerator()
            {
            }

            internal UnityDictionaryEnumerator(UnityDictionary<K, V> ud)
            {
                // Make a copy of the dictionary entries currently in the SimpleDictionary T.
                items = new KeyValuePair<K, V>[ud.Count];

                ud.CopyTo(items, 0);
            }

            object IEnumerator.Current
            {
                get { return Current; }
            }

            /// <summary>
            /// Gets the current.
            /// </summary>
            /// <value>The current.</value>
            public KeyValuePair<K, V> Current
            {
                get
                {
                    ValidateIndex();
                    return items[index];
                }
            }

            // Return the current dictionary entry.
            /// <summary>
            /// Gets the entry.
            /// </summary>
            /// <value>The entry.</value>
            public KeyValuePair<K, V> Entry
            {
                get { return (KeyValuePair<K, V>)Current; }
            }

            /// <summary>
            /// Disposes this instance.
            /// </summary>
            public void Dispose()
            {
                index = -1;
                items = null;
            }

            // Return the key of the current item.
            /// <summary>
            /// Gets the key.
            /// </summary>
            /// <value>The key.</value>
            public K Key
            {
                get
                {
                    ValidateIndex();
                    return items[index].Key;
                }
            }

            // Return the value of the current item.
            /// <summary>
            /// Gets the value.
            /// </summary>
            /// <value>The value.</value>
            public V Value
            {
                get
                {
                    ValidateIndex();
                    return items[index].Value;
                }
            }

            // Advance to the next item.
            /// <summary>
            /// Moves the next.
            /// </summary>
            /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
            public bool MoveNext()
            {
                if (index < items.Length - 1)
                {
                    index++;
                    return true;
                }
                return false;
            }

            // Validate the enumeration index and throw an exception if the index is out of range.
            private void ValidateIndex()
            {
                if (index < 0 || index >= items.Length)
                {
                    throw new System.InvalidOperationException("Enumerator is before or after the collection.");
                }
            }

            // Reset the index to restart the enumeration.
            /// <summary>
            /// Resets this instance.
            /// </summary>
            public void Reset()
            {
                index = -1;
            }

            #endregion IDictionary interface
        }
    }

    /// <summary>
    /// Class UnityDictionary.
    /// </summary>
    /// <typeparam name="V"></typeparam>
    /// <seealso cref="Lerp2API.Utility.UnityDictionary{System.String, V}" />
    public abstract class UnityDictionary<V> : UnityDictionary<string, V>
    {
    }
}