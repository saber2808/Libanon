using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;

namespace Libanon.Models.EntityConfiguration
{
    public class ISBNEntityConfiguration : EntityTypeConfiguration<ISBN>
    {
        public ISBNEntityConfiguration()
        {
            this.HasKey(x => x.Id);
            this.Property(s => s.ISBNCode).IsRequired();
        }
    }
}