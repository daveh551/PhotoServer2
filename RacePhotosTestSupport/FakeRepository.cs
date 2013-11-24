using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Highway.Data;
using Highway.Data.Contexts;

namespace RacePhotosTestSupport
{
    public class FakeRepository : Repository
    {
        public FakeRepository()
            : base(new InMemoryDataContext())
        {
            
        }
    }
    
}
