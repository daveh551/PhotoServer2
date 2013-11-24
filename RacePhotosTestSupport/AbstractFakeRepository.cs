using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PhotoServer.DataAccessLayer;
using PhotoServer.Domain;

namespace RacePhotosTestSupport
{
	public abstract class AbstractFakeRepository<T, TKey> :IRepository<T, TKey> where T : IEntity<TKey>
	{
		        protected List<T> data;


        protected List<T> addedData; 
        public AbstractFakeRepository()
        {
            data = new List<T>();
            addedData = new List<T>();
        }
        public void Add(T item)
        {
            data.Add(item);
            addedData.Add(item);
        }

        public void Remove(T item)
        {
            throw new NotImplementedException();
        }

        public IQueryable<T> FindAll()
        {
            return   data.AsQueryable();
        }

		public abstract T FindById(TKey id);


        public IQueryable<T> Find(Func<T, bool> predicate)
        {
	        return data.Where(predicate).AsQueryable();
        }

		public abstract int SaveChanges();
	}
}
