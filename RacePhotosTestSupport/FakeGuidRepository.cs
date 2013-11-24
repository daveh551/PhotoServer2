using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PhotoServer.DataAccessLayer;

namespace RacePhotosTestSupport
{
    class FakeGuidRepository<T>: AbstractFakeRepository<T,Guid> where T : PhotoServer.Domain.IEntity<Guid>
    {
       
        public FakeGuidRepository() :base()
        {
        }



        public  override T FindById(Guid id)
        {
	        return data.FirstOrDefault(r => r.Id == id);
        }

   

        public override int  SaveChanges()
        {

	        int numberOfChanges = 0;
            foreach (var item in addedData)
            {
                if (item.Id == default(Guid))
                {
                    item.Id = Guid.NewGuid();
	                numberOfChanges++;
                }
            }
            addedData.Clear();
	        return numberOfChanges;
        }
    }
}
