// [[Highway.Onramp.MVC]]
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.SessionState;

namespace $rootnamespace$.App_Architecture.Services.Core
{
    public class DictionaryEnumerator<TKey, TValue> : IDictionaryEnumerator, IDisposable
    {
        readonly IEnumerator<KeyValuePair<TKey, TValue>> enumerator;

        public DictionaryEnumerator(IEnumerator<KeyValuePair<TKey, TValue>> enumerator)
        {
            this.enumerator = enumerator;
        }

        public void Dispose()
        {
            enumerator.Dispose();
        }

        public bool MoveNext()
        {
            return enumerator.MoveNext();
        }

        public void Reset()
        {
            enumerator.Reset();
        }

        public object Current
        {
            get { return enumerator.Current; }
        }

        public object Key
        {
            get { return enumerator.Current.Key; }
        }

        public object Value
        {
            get { return enumerator.Current.Value; }
        }

        public DictionaryEntry Entry
        {
            get { return new DictionaryEntry(Key, Value); }
        }
    }
}
