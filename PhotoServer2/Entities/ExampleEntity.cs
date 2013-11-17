// [[Highway.Onramp.MVC.Data]]
using Highway.Data.PrebuiltQueries;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PhotoServer2.Entities
{
    public class ExampleEntity : BaseEntity
    {
        // Id is a Guid and inherited from BaseEntity
        public string Name { get; set; }
    }
}
