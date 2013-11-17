// [[Highway.Onramp.MVC.Data]]
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;

namespace $rootnamespace$.Entities.Mappings
{
    public abstract class BaseMapping<T> : EntityTypeConfiguration<T> where T : BaseEntity
    {
        public BaseMapping()
        {
            this.HasKey(e => e.Id);
            this.Property(e => e.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
        }
    }

}