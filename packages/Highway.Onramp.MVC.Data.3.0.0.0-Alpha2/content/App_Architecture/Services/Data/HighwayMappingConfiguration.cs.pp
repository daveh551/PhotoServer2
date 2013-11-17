// [[Highway.Onramp.MVC.Data]]
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Highway.Data;
using $rootnamespace$.Entities;

namespace $rootnamespace$.App_Architecture.Services.Data
{
    public class HighwayMappingConfiguration : IMappingConfiguration
    {
        public void ConfigureModelBuilder(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.AddFromAssembly(this.GetType().Assembly);
        }
    }
}