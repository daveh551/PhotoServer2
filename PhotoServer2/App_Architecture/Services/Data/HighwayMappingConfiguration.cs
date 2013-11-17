// [[Highway.Onramp.MVC.Data]]
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Highway.Data;
using PhotoServer2.Entities;

namespace PhotoServer2.App_Architecture.Services.Data
{
    public class HighwayMappingConfiguration : IMappingConfiguration
    {
        public void ConfigureModelBuilder(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.AddFromAssembly(this.GetType().Assembly);
        }
    }
}