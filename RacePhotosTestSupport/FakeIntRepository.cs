using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PhotoServer.DataAccessLayer;

namespace RacePhotosTestSupport
{
    public class FakeIntRepository<T>: AbstractFakeRepository<T,int> where T : PhotoServer.Domain.IEntity<int>
    {
	    private int _maxIndex;
       
        public FakeIntRepository() :base()
        {
	        _maxIndex = data.Any() ? data.Max(item => item.Id) : 0;
        }

		

        public  override T FindById(int id)
        {
	        return data.FirstOrDefault(r => r.Id == id);
        }

   

        public override int  SaveChanges()
        {

	        int numberOfChanges = 0;
            foreach (var item in addedData)
            {
                if (item.Id == default(int))
                {
	                item.Id = ++_maxIndex;
	                numberOfChanges++;
                }
            }
            addedData.Clear();
	        return numberOfChanges;
        }
    }
}
