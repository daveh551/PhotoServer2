// [[Highway.Onramp.MVC]]
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.SessionState;

namespace $rootnamespace$.App_Architecture.Services.Core
{
    public class CollectionAdapter<T> : ICollection<T>, ICollection
    {
        readonly ICollection collection;

        public void CopyTo(Array array, int index)
        {
            collection.CopyTo(array, index);
        }

        public object SyncRoot
        {
            get { return collection.SyncRoot; }
        }

        public bool IsSynchronized
        {
            get { return collection.IsSynchronized; }
        }

        public CollectionAdapter(ICollection collection)
        {
            this.collection = collection;
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return collection.Cast<T>().GetEnumerator();
        }

        public IEnumerator GetEnumerator()
        {
            return collection.GetEnumerator();
        }

        public void Add(T item)
        {
            throw new NotSupportedException();
        }

        public void Clear()
        {
            throw new NotSupportedException();
        }

        public bool Contains(T item)
        {
            return collection.Cast<T>().Any(x => Equals(x, item));
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            collection.CopyTo(array, arrayIndex);
        }

        public bool Remove(T item)
        {
            throw new NotSupportedException();
        }

        public int Count
        {
            get { return collection.Count; }
        }

        public bool IsReadOnly
        {
            get { return true; }
        }
    }
}
