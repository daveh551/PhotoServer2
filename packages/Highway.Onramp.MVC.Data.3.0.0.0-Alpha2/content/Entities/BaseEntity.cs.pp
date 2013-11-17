// [[Highway.Onramp.MVC.Data]]
using Highway.Data.PrebuiltQueries;
using System;
using System.Collections.Generic;
using System.Linq;

namespace $rootnamespace$.Entities
{
    public abstract class BaseEntity : IIdentifiable<Guid>
    {
        public Guid Id { get; set; }
    }
}
