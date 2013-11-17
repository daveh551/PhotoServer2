// [[Highway.Onramp.MVC]]
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.SessionState;

namespace $rootnamespace$.App_Architecture.Services.Core
{
    public class SessionDictionary : IDictionary<string, object>, IDictionary
    {
        readonly HttpSessionState sessionState;
        readonly CollectionAdapter<string> keysAdapter;
        readonly CollectionAdapter<object> valuesAdapter;

        public SessionDictionary(HttpSessionState sessionState)
        {
            this.sessionState = sessionState;
            keysAdapter = new CollectionAdapter<string>(sessionState.Keys);
            valuesAdapter = new CollectionAdapter<object>(sessionState);
        }

        public bool ContainsKey(string key)
        {
            return keysAdapter.Contains(key);
        }

        public void Add(string name, object value)
        {
            sessionState.Add(name, value);
        }

        public bool Remove(string name)
        {
            if (!ContainsKey(name)) return false;
            sessionState.Remove(name);
            return true;
        }

        public bool TryGetValue(string key, out object value)
        {
            if (ContainsKey(key))
            {
                value = this[key];
                return true;
            }

            value = null;
            return false;
        }

        public void Add(KeyValuePair<string, object> item)
        {
            Add(item.Key, item.Value);
        }

        public bool Contains(object key)
        {
            return ContainsKey((string)key);
        }

        public void Add(object key, object value)
        {
            Add((string)key, value);
        }

        public void Clear()
        {
            sessionState.Clear();
        }

        IDictionaryEnumerator IDictionary.GetEnumerator()
        {
            return new DictionaryEnumerator<string, object>(KeyValueEnumerable().GetEnumerator());
        }

        public void Remove(object key)
        {
            Remove((string)key);
        }

        object IDictionary.this[object key]
        {
            get { return this[(string)key]; }
            set { this[(string)key] = value; }
        }

        public bool Contains(KeyValuePair<string, object> item)
        {
            if (!ContainsKey(item.Key)) return false;
            return Equals(this[item.Key], item.Value);
        }

        public void CopyTo(KeyValuePair<string, object>[] array, int arrayIndex)
        {
            CopyTo((Array)array, arrayIndex);
        }

        public bool Remove(KeyValuePair<string, object> item)
        {
            if (!Contains(item)) return false;
            return Remove(item.Key);
        }

        IEnumerator<KeyValuePair<string, object>> IEnumerable<KeyValuePair<string, object>>.GetEnumerator()
        {
            return KeyValueEnumerable().GetEnumerator();
        }

        IEnumerable<KeyValuePair<string, object>> KeyValueEnumerable()
        {
            return Keys.Select(key => new KeyValuePair<string, object>(key, this[key]));
        }

        public IEnumerator GetEnumerator()
        {
            return sessionState.GetEnumerator();
        }

        public object this[string name]
        {
            get { return sessionState[name]; }
            set { sessionState[name] = value; }
        }

        public void CopyTo(Array array, int index)
        {
            sessionState.CopyTo(array, index);
        }

        public int Count
        {
            get { return sessionState.Count; }
        }

        public object SyncRoot
        {
            get { return sessionState.SyncRoot; }
        }

        public bool IsSynchronized
        {
            get { return sessionState.IsSynchronized; }
        }

        public ICollection<string> Keys
        {
            get { return keysAdapter; }
        }

        ICollection IDictionary.Values
        {
            get { return valuesAdapter; }
        }

        ICollection IDictionary.Keys
        {
            get { return keysAdapter; }
        }

        public ICollection<object> Values
        {
            get { return valuesAdapter; }
        }

        public bool IsReadOnly
        {
            get { return sessionState.IsReadOnly; }
        }

        public bool IsFixedSize
        {
            get { return false; }
        }
    }

}